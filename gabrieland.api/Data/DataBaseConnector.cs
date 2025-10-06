using gabrieland.api.Data;
using gabrieland.api.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Net.NetworkInformation;
namespace gabrieland.Data
{
    public class DataBaseConnector : ConnectionDb
    {
        public DataBaseConnector(IConfiguration configuration) : base(configuration) { }
        // Login methods
        public async Task<Usuario> AuthenticateUsuario(string userName, string contraseña)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_Usuario"", ""nombre"", ""apellido"", ""correo"", 
                                 ""num_telefonico"", ""Hash_Contraseña"", ""fecha_nacimiento"", 
                                 ""Activo"", ""TUO_ID_Tipo_Usuario"" FROM ""Usuarios"" 
                                 WHERE ""Username"" = :userName AND ""Activo"" = 'Y'";
                    command.Parameters.Add(new OracleParameter("Username", userName));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var usuario = new Usuario
                            {
                                ID_Usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellido = reader.IsDBNull(2) ? null : reader.GetString(2),
                                correo = reader.GetString(3),
                                num_telefonico = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Hash_Contraseña = reader.GetString(5),
                                fecha_nacimiento = reader.GetDateTime(6),
                                Activo = reader.GetString(7),
                                TUO_ID_Tipo_Usuario = reader.GetInt32(8)
                            };

                            // Verify password (implementation shown below)
                            if (contraseña == usuario.Hash_Contraseña)
                            {
                                // Remove sensitive data before returning
                                usuario.Hash_Contraseña = null;
                                return usuario;
                            }
                        }
                    }
                }
            }
            return null;
        }
        public async Task<bool> UsuarioExists(string userName)//Using StoreProcedure
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "check_user_exists";

                    // Correct way to add input parameter:
                    command.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2, userName, ParameterDirection.Input));

                    // Add output parameter
                    command.Parameters.Add(new OracleParameter("p_exists", OracleDbType.Int32, ParameterDirection.Output));

                    await command.ExecuteNonQueryAsync();

                    var oracleDecimal = (OracleDecimal)command.Parameters["p_exists"].Value;
                    return oracleDecimal.ToInt32() > 0;
                }
            }
        }

        public async Task<bool> CorreoExists(string correo)
        {
           using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT COUNT(*) FROM ""Usuarios"" WHERE ""correo"" = :correo";
                    command.Parameters.Add(new OracleParameter("correo", correo));

                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }
        // Metodos para tipos de usuario
        public async Task<List<TiposUsuarios>> GetTiposUsuarios()
        {
            var result = new List<TiposUsuarios>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_Tipos_usuario"", ""nombre"" 
                                 FROM ""Tipos_Usuarios"" 
                                 ORDER BY ""ID_Tipos_usuario""";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new TiposUsuarios
                            {
                                ID_Tipos_usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<TiposUsuarios> GetTipoUsuarioById(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_Tipos_usuario"", ""nombre"" 
                                 FROM ""Tipos_Usuarios"" 
                                 WHERE ""ID_Tipos_usuario"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new TiposUsuarios
                            {
                                ID_Tipos_usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1)
                            };
                        }
                    }
                }
            }
            return null;
        }
        public async Task<int> CreateTipoUsuario(TiposUsuarios tipoUsuario)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO ""Tipos_Usuarios"" (""nombre"") 
                                VALUES (:nombre)
                                RETURNING ""ID_Tipos_usuario"" INTO :id";

                    command.Parameters.Add(new OracleParameter("nombre", tipoUsuario.nombre));

                    var idParam = new OracleParameter("id", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();

                    return Convert.ToInt32(idParam.Value.ToString());
                }
            }
        }
        public async Task<bool> UpdateTipoUsuario(TiposUsuarios tipoUsuario)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Tipos_Usuarios"" SET
                                ""nombre"" = :nombre
                                WHERE ""ID_Tipos_usuario"" = :id";

                    command.Parameters.Add(new OracleParameter("nombre", tipoUsuario.nombre));
                    command.Parameters.Add(new OracleParameter("id", tipoUsuario.ID_Tipos_usuario));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        public async Task<bool> DeleteTipoUsuario(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM ""Tipos_Usuarios"" 
                                WHERE ""ID_Tipos_usuario"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        public async Task<bool> IsTipoUsuarioInUse(int tipoUsuarioId)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT COUNT(*) FROM ""Usuarios"" 
                                 WHERE ""TUO_ID_Tipo_Usuario"" = :tipoUsuarioId";
                    command.Parameters.Add(new OracleParameter("tipoUsuarioId", tipoUsuarioId));

                    var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }
        //Metodos para Usuarios
        public async Task<List<Usuario>> GetUsuarios(bool incluirInactivos = false)
        {
            var result = new List<Usuario>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    var query = @"SELECT ""ID_Usuario"", ""nombre"", ""apellido"", ""correo"", 
                        ""num_telefonico"", ""Hash_Contraseña"", ""fecha_nacimiento"", 
                        ""Activo"", ""TUO_ID_Tipo_Usuario"",""Username"" FROM ""Usuarios""";

                    if (!incluirInactivos)
                    {
                        query += " WHERE \"Activo\" = 'Y'";
                    }

                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new Usuario
                            {
                                ID_Usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellido = reader.IsDBNull(2) ? null : reader.GetString(2),
                                correo = reader.GetString(3),
                                num_telefonico = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Hash_Contraseña = null,
                                fecha_nacimiento = reader.GetDateTime(6),
                                Activo = reader.GetString(7),
                                TUO_ID_Tipo_Usuario = reader.GetInt32(8),
                                UserName = reader.GetString(9)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<Usuario> GetUsuarioById(int id, bool incluirInactivos = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    var query = @"SELECT ""ID_Usuario"", ""nombre"", ""apellido"", ""correo"", 
                        ""num_telefonico"", ""Hash_Contraseña"", ""fecha_nacimiento"", 
                        ""Activo"", ""TUO_ID_Tipo_Usuario"",""Username"" FROM ""Usuarios"" 
                        WHERE ""ID_Usuario"" = :id";

                    if (!incluirInactivos)
                    {
                        query += " AND \"Activo\" = 'Y'";
                    }

                    command.CommandText = query;
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Usuario
                            {
                                ID_Usuario = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellido = reader.IsDBNull(2) ? null : reader.GetString(2),
                                correo = reader.GetString(3),
                                num_telefonico = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Hash_Contraseña = reader.GetString(5),
                                fecha_nacimiento = reader.GetDateTime(6),
                                Activo = reader.GetString(7),
                                TUO_ID_Tipo_Usuario = reader.GetInt32(8),
                                UserName = reader.GetString(9)
                            };
                        }
                    }
                }
            }
            return null;
        }
        public async Task<int> CreateUsuario(Usuario usuario) // Using StoreProcedure
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "create_usuario";

                    // Input parameters
                    command.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2, usuario.nombre, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_apellido", OracleDbType.Varchar2, usuario.apellido ?? (object)DBNull.Value, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_correo", OracleDbType.Varchar2, usuario.correo, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_num_telefonico", OracleDbType.Varchar2, usuario.num_telefonico ?? (object)DBNull.Value, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_hash_contraseña", OracleDbType.Varchar2, usuario.Hash_Contraseña, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_fecha_nacimiento", OracleDbType.Date, usuario.fecha_nacimiento, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_activo", OracleDbType.Varchar2, usuario.Activo ?? "Y", ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_tuo_id_tipo_usuario", OracleDbType.Int32, usuario.TUO_ID_Tipo_Usuario, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2, usuario.UserName, ParameterDirection.Input));

                    // Output parameter for the ID
                    var idParam = new OracleParameter("p_id_usuario", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();

                    // Handle OracleDecimal conversion safely
                    return Convert.ToInt32(((OracleDecimal)idParam.Value).ToString());
                }
            }
        }
        public async Task<bool> UpdateUsuario(Usuario usuario) // Using StoreProcedure
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "update_usuario";

                    // Input parameters
                    command.Parameters.Add(new OracleParameter("p_id_usuario", OracleDbType.Int32, usuario.ID_Usuario, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2, usuario.nombre, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_apellido", OracleDbType.Varchar2, usuario.apellido ?? (object)DBNull.Value, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_correo", OracleDbType.Varchar2, usuario.correo, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_num_telefonico", OracleDbType.Varchar2, usuario.num_telefonico ?? (object)DBNull.Value, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_hash_contraseña", OracleDbType.Varchar2, usuario.Hash_Contraseña, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_fecha_nacimiento", OracleDbType.Date, usuario.fecha_nacimiento, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_activo", OracleDbType.Varchar2, usuario.Activo, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_tuo_id_tipo_usuario", OracleDbType.Int32, usuario.TUO_ID_Tipo_Usuario, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_username", OracleDbType.Varchar2, usuario.UserName, ParameterDirection.Input));

                    // Output parameter for rows affected
                    var rowsUpdatedParam = new OracleParameter("p_rows_updated", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(rowsUpdatedParam);

                    await command.ExecuteNonQueryAsync();

                    // Convert OracleDecimal to int safely
                    return Convert.ToInt32(rowsUpdatedParam.Value.ToString()) > 0;
                }
            }
        }
        public async Task<bool> LogicalDeleteUsuario(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Usuarios"" SET 
                                 ""Activo"" = 'N' 
                                 WHERE ""ID_Usuario"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        public async Task<bool> ReactivateUsuario(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Usuarios"" SET 
                                 ""Activo"" = 'Y' 
                                 WHERE ""ID_Usuario"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        // Metodos Para Salas
        public async Task<List<Salas>> GetSalas(bool incluirInactivos = false)
        {
            var result = new List<Salas>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    var query = @"SELECT ""ID_Sala"", ""nombre"", ""descripcion"", ""capacidad"", ""precio"", ""Activo"" 
                        FROM ""Salas""";

                    if (!incluirInactivos)
                    {
                        query += " WHERE \"Activo\" = 'Y'";
                    }

                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new Salas
                            {
                                id_sala = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Capacidad = reader.GetInt32(3),
                                Precio = reader.GetFloat(4),
                                Activo = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<Salas> GetSalaById(int id, bool incluirInactivos = false)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    var query = @"SELECT ""ID_Sala"", ""nombre"", ""descripcion"", ""capacidad"", ""precio"", ""Activo"" 
                        FROM ""Salas"" 
                        WHERE ""ID_Sala"" = :id";

                    if (!incluirInactivos)
                    {
                        query += " AND \"Activo\" = 'Y'";
                    }

                    command.CommandText = query;
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Salas
                            {
                                id_sala = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                Capacidad = reader.GetInt32(3),
                                Precio = reader.GetFloat(4),
                                Activo = reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return null;
        }
        public async Task<int> CreateSala(Salas sala)// Using StoreProcedures
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "create_sala";

                    // Input parameters
                    command.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2, sala.nombre, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_descripcion", OracleDbType.Varchar2, sala.Descripcion, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_capacidad", OracleDbType.Int32, sala.Capacidad, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_precio", OracleDbType.Decimal, sala.Precio, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_activo", OracleDbType.Varchar2, sala.Activo, ParameterDirection.Input));

                    // Output parameter for the ID
                    var idParam = new OracleParameter("p_id_sala", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();

                    // Safe conversion from OracleDecimal to int
                    return ((OracleDecimal)idParam.Value).ToInt32();
                }
            }
        }
        public async Task<bool> UpdateSala(Salas sala)// using StoreProcedures
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "update_sala";

                    // Input parameters
                    command.Parameters.Add(new OracleParameter("p_id_sala", OracleDbType.Int32, sala.id_sala, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2, 100, sala.nombre, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_descripcion", OracleDbType.Varchar2, 500,
                        sala.Descripcion ?? (object)DBNull.Value, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_capacidad", OracleDbType.Int32, sala.Capacidad, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_precio", OracleDbType.Decimal, sala.Precio, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_activo", OracleDbType.Varchar2, 1, sala.Activo, ParameterDirection.Input));

                    // Output parameter (simple boolean flag)
                    var successParam = new OracleParameter("p_success", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(successParam);

                    await command.ExecuteNonQueryAsync();

                    // Direct boolean conversion (1 = true, 0 = false)
                    // return Convert.ToInt32(rowsUpdatedParam.Value.ToString()) > 0;

                    return Convert.ToInt32(successParam.Value.ToString()) == 1;
                }
            }
        }
        public async Task<bool> LogicalDeleteSala(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Salas"" SET 
                                 ""Activo"" = 'N' 
                                 WHERE ""ID_Sala"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        public async Task<bool> ReactivateSala(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Salas"" SET 
                                 ""Activo"" = 'Y' 
                                 WHERE ""ID_Sala"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        // Methodos de fotos
        public async Task<int> AddFotoSala(FotoSala fotoSala)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO ""Fotos_Salas"" (""foto"", ""SAL_ID_Sala"",""es_principal"") 
                                 VALUES (:foto, :salaId, :es_principal)
                                 RETURNING ""ID_foto"" INTO :id";
                    command.Parameters.Add(new OracleParameter("foto", fotoSala.foto));
                    command.Parameters.Add(new OracleParameter("salaId", fotoSala.SAL_ID_Sala));
                    command.Parameters.Add(new OracleParameter("es_principal", fotoSala.esPrincipal));


                    var idParam = new OracleParameter("id", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();
                    return Convert.ToInt32(idParam.Value.ToString());
                }
            }
        }
        public async Task<List<FotoSala>> GetFotosBySalaId(int salaId)
        {
            var result = new List<FotoSala>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_foto"", ""foto"", ""SAL_ID_Sala"" , ""es_principal""
                                 FROM ""Fotos_Salas"" 
                                 WHERE ""SAL_ID_Sala"" = :salaId";
                    command.Parameters.Add(new OracleParameter("salaId", salaId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new FotoSala
                            {
                                ID_foto = reader.GetInt32(0),
                                foto = reader.GetString(1),
                                SAL_ID_Sala = reader.GetInt32(2),
                                esPrincipal = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return result;
        }

        //Todas las fotos
        public async Task<List<FotoSala>> GetTodasLasFotosAsync()
        {
            var result = new List<FotoSala>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_foto"", ""foto"", ""SAL_ID_Sala"", ""es_principal""
                                            FROM ""Fotos_Salas""";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new FotoSala
                            {
                                ID_foto = reader.GetInt32(0),
                                foto = reader.GetString(1),
                                SAL_ID_Sala = reader.GetInt32(2),
                                esPrincipal = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<FotoSala> GetSalaFotobyId(int fotoId)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_foto"", ""foto"", ""SAL_ID_Sala"", ""es_principal""
                                 FROM ""Fotos_Salas"" 
                                 WHERE ""ID_foto"" = :fotoId";
                    command.Parameters.Add(new OracleParameter("fotoId", fotoId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new FotoSala
                            {
                                ID_foto = reader.GetInt32(0),
                                foto = reader.GetString(1),
                                SAL_ID_Sala = reader.GetInt32(2),
                                esPrincipal = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null; // Or throw an exception if not found
        }
        public async Task<bool> DeleteFotoSala(int fotoId)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM ""Fotos_Salas"" 
                                     WHERE ""ID_foto"" = :fotoId";
                    command.Parameters.Add(new OracleParameter("fotoId", fotoId));

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting foto: {ex.Message}");
                throw;
            }
        }
        //Methodos Para tipos de Servicio
        public async Task<List<TiposServicios>> GetTiposServicios()
        {
            var result = new List<TiposServicios>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_Tipo_Servicio"", ""nombre"", ""incluido"" 
                                 FROM ""Tipos_Servicios"" 
                                 ORDER BY ""ID_Tipo_Servicio""";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new TiposServicios
                            {
                                ID_Tipo_Servicio = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                incluido = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<TiposServicios> GetTipoServicioById(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_Tipo_Servicio"", ""nombre"", ""incluido"" 
                                 FROM ""Tipos_Servicios"" 
                                 WHERE ""ID_Tipo_Servicio"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new TiposServicios
                            {
                                ID_Tipo_Servicio = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                incluido = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }
        public async Task<int> CreateTipoServicio(TiposServicios tipoServicio)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO ""Tipos_Servicios"" 
                                (""nombre"", ""incluido"") 
                                VALUES (:nombre, :incluido)
                                RETURNING ""ID_Tipo_Servicio"" INTO :id";

                    command.Parameters.Add(new OracleParameter("nombre", tipoServicio.nombre));
                    command.Parameters.Add(new OracleParameter("incluido", tipoServicio.incluido));

                    var idParam = new OracleParameter("id", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();

                    return Convert.ToInt32(idParam.Value.ToString());
                }
            }
        }
        public async Task<bool> UpdateTipoServicio(TiposServicios tipoServicio)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Tipos_Servicios"" SET
                                ""nombre"" = :nombre,
                                ""incluido"" = :incluido
                                WHERE ""ID_Tipo_Servicio"" = :id";

                    command.Parameters.Add(new OracleParameter("nombre", tipoServicio.nombre));
                    command.Parameters.Add(new OracleParameter("incluido", tipoServicio.incluido));
                    command.Parameters.Add(new OracleParameter("id", tipoServicio.ID_Tipo_Servicio));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        public async Task<bool> DeleteTipoServicio(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM ""Tipos_Servicios"" 
                                WHERE ""ID_Tipo_Servicio"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        //Metodos para Servicios
        public async Task<List<ServicioAdicional>> GetServiciosAdicionales(bool incluirInactivos = false)
        {
            var result = new List<ServicioAdicional>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    var query = @"SELECT sa.""ID_Servicios_Adicionales"", sa.""nombre"", sa.""descripcion"", 
                        sa.""costo"", sa.""Activo"", sa.""TSO_ID_Tipo_Servicio"",
                        ts.""nombre"" as TipoServicioNombre
                        FROM ""Servicios_Adicionales"" sa
                        JOIN ""Tipos_Servicios"" ts ON sa.""TSO_ID_Tipo_Servicio"" = ts.""ID_Tipo_Servicio""";

                    if (!incluirInactivos)
                    {
                        query += " WHERE sa.\"Activo\" = 'Y'";
                    }

                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new ServicioAdicional
                            {
                                ID_Servicios_Adicionales = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                costo = reader.GetFloat(3),
                                Activo = reader.GetString(4),
                                TSO_ID_Tipo_Servicio = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<ServicioAdicional> GetServicioAdicionalById(int id, bool incluirInactivos = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    var query = @"SELECT ""ID_Servicios_Adicionales"", ""nombre"", ""descripcion"", 
                        ""costo"", ""Activo"", ""TSO_ID_Tipo_Servicio""
                        FROM ""Servicios_Adicionales"" 
                        WHERE ""ID_Servicios_Adicionales"" = :id";

                    if (!incluirInactivos)
                    {
                        query += " AND \"Activo\" = 'Y'";
                    }

                    command.CommandText = query;
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ServicioAdicional
                            {
                                ID_Servicios_Adicionales = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                                costo = reader.GetFloat(3),
                                Activo = reader.GetString(4),
                                TSO_ID_Tipo_Servicio = reader.GetInt32(5),
                            };
                        }
                    }
                }
            }
            return null;
        }
        public async Task<int> CreateServicioAdicional(ServicioAdicional servicio)// using StoreProcedures
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "create_servicio_adicional";

                    // Input parameters with explicit types and sizes
                    command.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2, 100, servicio.nombre, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_descripcion", OracleDbType.Varchar2, 500,
                        servicio.descripcion ?? (object)DBNull.Value, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_costo", OracleDbType.Decimal, servicio.costo, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_activo", OracleDbType.Varchar2, 1,
                        servicio.Activo ?? "Y", ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_tso_id_tipo_servicio", OracleDbType.Int32,
                        servicio.TSO_ID_Tipo_Servicio, ParameterDirection.Input));

                    // Output parameter
                    var idParam = new OracleParameter("p_id_servicio", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();

                    // Safe conversion from OracleDecimal
                    return ((OracleDecimal)idParam.Value).ToInt32();
                }
            }
        }
        public async Task<bool> UpdateServicioAdicional(ServicioAdicional servicio)// using StoreProcedures
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "update_servicio_adicional";

                    // Input parameters with explicit types and sizes
                    command.Parameters.Add(new OracleParameter("p_id_servicio", OracleDbType.Int32,
                        servicio.ID_Servicios_Adicionales, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_nombre", OracleDbType.Varchar2, 100,
                        servicio.nombre, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_descripcion", OracleDbType.Varchar2, 500,
                        servicio.descripcion ?? (object)DBNull.Value, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_costo", OracleDbType.Decimal,
                        servicio.costo, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_activo", OracleDbType.Varchar2, 1,
                        servicio.Activo, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_tipo_servicio", OracleDbType.Int32,
                        servicio.TSO_ID_Tipo_Servicio, ParameterDirection.Input));

                    // Output parameter
                    var successParam = new OracleParameter("p_success", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(successParam);

                    await command.ExecuteNonQueryAsync();

                    // Convert to boolean (1 = true, 0 = false)
                    return Convert.ToInt32(successParam.Value.ToString()) == 1;
                }
            }
        }
        public async Task<bool> LogicalDeleteServicioAdicional(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Servicios_Adicionales"" SET 
                                 ""Activo"" = 'N' 
                                 WHERE ""ID_Servicios_Adicionales"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        public async Task<bool> ReactivateServicioAdicional(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Servicios_Adicionales"" SET 
                                 ""Activo"" = 'Y' 
                                 WHERE ""ID_Servicios_Adicionales"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        // Methodos de fotos
        public async Task<int> AddFotoServicio(FotoServicio fotoServicio)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO ""Fotos_Servicios"" (""foto"", ""SAL_ID_Servicio"", ""es_principal"") 
                                 VALUES (:foto, :servicioId, :es_principal)
                                 RETURNING ""ID_foto"" INTO :id";
                    command.Parameters.Add(new OracleParameter("foto", fotoServicio.foto));
                    command.Parameters.Add(new OracleParameter("servicioId", fotoServicio.SAL_ID_Servicio));
                    command.Parameters.Add(new OracleParameter("es_principal", fotoServicio.esPrincipal));

                    var idParam = new OracleParameter("id", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();
                    return Convert.ToInt32(idParam.Value.ToString());
                }
            }
        }

        public async Task<List<FotoServicio>> GetTodasLasFotosServiciosAsync()
        {
            var result = new List<FotoServicio>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_foto"", ""foto"", ""SAL_ID_Servicio"", ""es_principal""
                                            FROM ""Fotos_Servicios""";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new FotoServicio
                            {
                                ID_foto = reader.GetInt32(0),
                                foto = reader.GetString(1),
                                SAL_ID_Servicio = reader.GetInt32(2),
                                esPrincipal = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<List<FotoServicio>> GetFotosByServicioId(int servicioId)
        {
            var result = new List<FotoServicio>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_foto"", ""foto"", ""SAL_ID_Servicio"", ""es_principal""
                                 FROM ""Fotos_Servicios"" 
                                 WHERE ""SAL_ID_Servicio"" = :servicioId";
                    command.Parameters.Add(new OracleParameter("servicioId", servicioId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new FotoServicio
                            {
                                ID_foto = reader.GetInt32(0),
                                foto = reader.GetString(1),
                                SAL_ID_Servicio = reader.GetInt32(2),
                                esPrincipal = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<FotoServicio> GetServicioFotobyId(int fotoId)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT ""ID_foto"", ""foto"", ""SAL_ID_Servicio"", ""es_principal""
                                 FROM ""Fotos_Servicios"" 
                                 WHERE ""ID_foto"" = :fotoId";
                    command.Parameters.Add(new OracleParameter("fotoId", fotoId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new FotoServicio
                            {
                                ID_foto = reader.GetInt32(0),
                                foto = reader.GetString(1),
                                SAL_ID_Servicio = reader.GetInt32(2),
                                esPrincipal = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }
        public async Task<bool> DeleteFotoServicio(int fotoId)
        {
            try
            {
                using var connection = new OracleConnection(_connectionString);
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM ""Fotos_Servicios"" 
                                 WHERE ""ID_foto"" = :fotoId";
                    command.Parameters.Add(new OracleParameter("fotoId", fotoId));

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting servicio photo: {ex.Message}");
                throw;
            }
        }

    }
}
