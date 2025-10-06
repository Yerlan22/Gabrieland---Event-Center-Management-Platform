using gabrieland.api.Data;
using gabrieland.api.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace gabrieland.api.Data
{
    public class ReservasData : ConnectionDb
    {
        public ReservasData(IConfiguration configuration) : base(configuration) { }
        public async Task<List<Reserva>> GetReservas(string? estado)
        {
            var result = new List<Reserva>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    var query = @"SELECT ""ID_Reserva"", ""duracion"", ""estado"", 
                        ""fecha"", ""SAL_ID_Sala"", ""USU_ID_Usuario""
                        FROM ""Reservas""";

                    if (estado != null)
                    {
                        query += " WHERE \"estado\" = :estado";
                        command.Parameters.Add(new OracleParameter("estado", estado));
                    }

                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new Reserva
                            {
                                IdReserva = reader.GetInt32(0),
                                Duracion = reader.GetInt32(1),
                                Estado = Enum.Parse<EstadoReserva>(reader.GetString(2)),
                                Fecha = reader.GetDateTime(3),
                                SalaId = reader.GetInt32(4),
                                UsuarioId = reader.GetInt32(5),
                            });
                        }
                    }
                }
            }
            return result;
        }
        public async Task<List<Reserva>> GetReservasByUsuario(int usuarioId)
        {
            var lista = new List<Reserva>();

            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    ""ID_Reserva"", 
                    ""duracion"", 
                    ""estado"", 
                    ""fecha"", 
                    ""SAL_ID_Sala"", 
                    ""USU_ID_Usuario""
                FROM ""Reservas""
                WHERE ""USU_ID_Usuario"" = :usuarioId";

            command.Parameters.Add(new OracleParameter("usuarioId", usuarioId));

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var reserva = new Reserva
                {
                    IdReserva = reader.GetInt32(0),
                    Duracion = reader.GetInt32(1),
                    Estado = Enum.Parse<EstadoReserva>(reader.GetString(2)),
                    Fecha = reader.GetDateTime(3),
                    SalaId = reader.GetInt32(4),
                    UsuarioId = reader.GetInt32(5),
                };
                lista.Add(reserva);
            }

            return lista;
        }

        public async Task<Reserva?> GetReservaById(int id)
        {
            using var connection = new OracleConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
        SELECT 
            ""ID_Reserva"", 
            ""duracion"", 
            ""estado"", 
            ""fecha"", 
            ""SAL_ID_Sala"", 
            ""USU_ID_Usuario""
        FROM ""Reservas""
        WHERE ""ID_Reserva"" = :id";

            command.Parameters.Add(new OracleParameter("id", id));

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Reserva
                {
                    IdReserva = reader.GetInt32(0),
                    Duracion = reader.GetInt32(1),
                    Estado = Enum.Parse<EstadoReserva>(reader.GetString(2)),
                    Fecha = reader.GetDateTime(3),
                    SalaId = reader.GetInt32(4),
                    UsuarioId = reader.GetInt32(5),
                };
            }

            return null;
        }
        public async Task<int> CreateReserva(Reserva reserva)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO ""Reservas"" 
                                        (""duracion"", ""estado"", ""fecha"", ""SAL_ID_Sala"", ""USU_ID_Usuario"")
                                        VALUES (:duracion, :estado, :fecha, :salaId, :usuarioId)
                                        RETURNING ""ID_Reserva"" INTO :id";

                    command.Parameters.Add(new OracleParameter("duracion", reserva.Duracion));
                    command.Parameters.Add(new OracleParameter("estado", reserva.Estado.ToString()));
                    command.Parameters.Add(new OracleParameter("fecha", reserva.Fecha));
                    command.Parameters.Add(new OracleParameter("salaId", reserva.SalaId));
                    command.Parameters.Add(new OracleParameter("usuarioId", reserva.UsuarioId));

                    var idParam = new OracleParameter("id", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();

                    return Convert.ToInt32(((OracleDecimal)idParam.Value).ToString());
                }
            }
        }
        public async Task<bool> UpdateReserva(Reserva reserva)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Reservas"" SET 
                                        ""duracion"" = :duracion,
                                        ""estado"" = :estado,
                                        ""fecha"" = :fecha,
                                        ""SAL_ID_Sala"" = :salaId,
                                        ""USU_ID_Usuario"" = :usuarioId
                                        WHERE ""ID_Reserva"" = :id";

                    command.Parameters.Add(new OracleParameter("duracion", reserva.Duracion));
                    command.Parameters.Add(new OracleParameter("estado", reserva.Estado.ToString()));
                    command.Parameters.Add(new OracleParameter("fecha", reserva.Fecha));
                    command.Parameters.Add(new OracleParameter("salaId", reserva.SalaId));
                    command.Parameters.Add(new OracleParameter("usuarioId", reserva.UsuarioId));
                    command.Parameters.Add(new OracleParameter("id", reserva.IdReserva));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
        public async Task<bool> LogicalDeleteReserva(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE ""Reservas"" SET 
                                        ""estado"" = 'Cancelada'
                                        WHERE ""ID_Reserva"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }
}