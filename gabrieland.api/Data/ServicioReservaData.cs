using gabrieland.api.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace gabrieland.api.Data
{
    public class ServicioReservaData : ConnectionDb
    {
        public ServicioReservaData(IConfiguration configuration) : base(configuration) { }

        public async Task<List<ServicioReserva>> GetAll()
        {
            var servicios = new List<ServicioReserva>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT ""ID"", ""cantidad"", ""Precio_pago"", 
                               ""RES_ID_Reserva"", ""SAE_ID_servicio""
                        FROM ""Servicios_reservas""";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            servicios.Add(new ServicioReserva
                            {
                                ID = reader.GetInt32(0),
                                Cantidad = reader.GetInt32(1),
                                PrecioPago = reader.GetDecimal(2),
                                ReservaId = reader.GetInt32(3),
                                ServicioId = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return servicios;
        }

        public async Task<ServicioReserva?> GetById(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT ""ID"", ""cantidad"", ""Precio_pago"", 
                               ""RES_ID_Reserva"", ""SAE_ID_servicio""
                        FROM ""Servicios_reservas""
                        WHERE ""ID"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ServicioReserva
                            {
                                ID = reader.GetInt32(0),
                                Cantidad = reader.GetInt32(1),
                                PrecioPago = reader.GetDecimal(2),
                                ReservaId = reader.GetInt32(3),
                                ServicioId = reader.GetInt32(4)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<ServicioReserva>> GetByReservaId(int reservaId)
        {
            var servicios = new List<ServicioReserva>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT ""ID"", ""cantidad"", ""Precio_pago"", 
                               ""RES_ID_Reserva"", ""SAE_ID_servicio""
                        FROM ""Servicios_reservas""
                        WHERE ""RES_ID_Reserva"" = :reservaId";
                    command.Parameters.Add(new OracleParameter("reservaId", reservaId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            servicios.Add(new ServicioReserva
                            {
                                ID = reader.GetInt32(0),
                                Cantidad = reader.GetInt32(1),
                                PrecioPago = reader.GetDecimal(2),
                                ReservaId = reader.GetInt32(3),
                                ServicioId = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return servicios;
        }

        public async Task<int> Create(ServicioReserva servicioReserva)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO ""Servicios_reservas"" 
                        (""cantidad"", ""Precio_pago"", ""RES_ID_Reserva"", ""SAE_ID_servicio"")
                        VALUES (:cantidad, :precio, :reservaId, :servicioId)
                        RETURNING ""ID"" INTO :id";

                    command.Parameters.Add(new OracleParameter("cantidad", servicioReserva.Cantidad));
                    command.Parameters.Add(new OracleParameter("precio", servicioReserva.PrecioPago));
                    command.Parameters.Add(new OracleParameter("reservaId", servicioReserva.ReservaId));
                    command.Parameters.Add(new OracleParameter("servicioId", servicioReserva.ServicioId));

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

        public async Task<bool> Update(ServicioReserva servicioReserva)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE ""Servicios_reservas"" SET 
                            ""cantidad"" = :cantidad,
                            ""Precio_pago"" = :precio,
                            ""RES_ID_Reserva"" = :reservaId,
                            ""SAE_ID_servicio"" = :servicioId
                        WHERE ""ID"" = :id";

                    command.Parameters.Add(new OracleParameter("cantidad", servicioReserva.Cantidad));
                    command.Parameters.Add(new OracleParameter("precio", servicioReserva.PrecioPago));
                    command.Parameters.Add(new OracleParameter("reservaId", servicioReserva.ReservaId));
                    command.Parameters.Add(new OracleParameter("servicioId", servicioReserva.ServicioId));
                    command.Parameters.Add(new OracleParameter("id", servicioReserva.ID));

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
                    command.CommandText = @"DELETE FROM ""Servicios_reservas"" WHERE ""ID"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    return await command.ExecuteNonQueryAsync() > 0;
                }
            }
        }
    }
}