using gabrieland.api.Models;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace gabrieland.api.Data
{
    public class TiposPagoData : ConnectionDb
    {
        public TiposPagoData(IConfiguration configuration) : base(configuration) { }

        public async Task<List<TipoPago>> GetAll()
        {
            var tipos = new List<TipoPago>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT ""ID_Tipo_pago"", ""nombre"", ""descripcion"" 
                        FROM ""Tipos_Pagos""";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            tipos.Add(new TipoPago
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return tipos;
        }

        public async Task<TipoPago?> GetById(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT ""ID_Tipo_pago"", ""nombre"", ""descripcion"" 
                        FROM ""Tipos_Pagos""
                        WHERE ""ID_Tipo_pago"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new TipoPago
                            {
                                Id = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<int> Create(TipoPago tipoPago)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO ""Tipos_Pagos"" 
                        (""nombre"", ""descripcion"")
                        VALUES (:nombre, :descripcion)
                        RETURNING ""ID_Tipo_pago"" INTO :id";

                    command.Parameters.Add(new OracleParameter("nombre", tipoPago.Nombre));
                    command.Parameters.Add(new OracleParameter("descripcion",
                        string.IsNullOrEmpty(tipoPago.Descripcion) ? (object)DBNull.Value : tipoPago.Descripcion));

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

        public async Task<bool> Update(TipoPago tipoPago)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE ""Tipos_Pagos"" SET 
                            ""nombre"" = :nombre,
                            ""descripcion"" = :descripcion
                        WHERE ""ID_Tipo_pago"" = :id";

                    command.Parameters.Add(new OracleParameter("nombre", tipoPago.Nombre));
                    command.Parameters.Add(new OracleParameter("descripcion",
                        string.IsNullOrEmpty(tipoPago.Descripcion) ? (object)DBNull.Value : tipoPago.Descripcion));
                    command.Parameters.Add(new OracleParameter("id", tipoPago.Id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM ""Tipos_Pagos"" WHERE ""ID_Tipo_pago"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }
}