using gabrieland.api.Models;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;

namespace gabrieland.api.Data
{
    public class FacturaData : ConnectionDb
    {
        public FacturaData(IConfiguration configuration) : base(configuration) { }

        public async Task<List<Factura>> GetAll()
        {
            var facturas = new List<Factura>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT f.""ID_Factura"", f.""fecha_emision"", f.""monto_sala"", 
                               f.""monto_servicios_adicionales"", f.""RES_ID_Reserva"", 
                               f.""TPO_ID_Tipo_pago"", t.""nombre"" as TipoPagoNombre
                        FROM ""Facturas"" f
                        JOIN ""Tipos_Pagos"" t ON f.""TPO_ID_Tipo_pago"" = t.""ID_Tipo_pago""";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            facturas.Add(new Factura
                            {
                                Id = reader.GetInt32(0),
                                FechaEmision = reader.GetDateTime(1),
                                MontoSala = reader.GetDecimal(2),
                                MontoServiciosAdicionales = reader.GetDecimal(3),
                                ReservaId = reader.GetInt32(4),
                                TipoPagoId = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return facturas;
        }

        public async Task<Factura?> GetById(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT f.""ID_Factura"", f.""fecha_emision"", f.""monto_sala"", 
                               f.""monto_servicios_adicionales"", f.""RES_ID_Reserva"", 
                               f.""TPO_ID_Tipo_pago"", t.""nombre"", t.""descripcion""
                        FROM ""Facturas"" f
                        JOIN ""Tipos_Pagos"" t ON f.""TPO_ID_Tipo_pago"" = t.""ID_Tipo_pago""
                        WHERE f.""ID_Factura"" = :id";
                    command.Parameters.Add(new OracleParameter("id", id));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Factura
                            {
                                Id = reader.GetInt32(0),
                                FechaEmision = reader.GetDateTime(1),
                                MontoSala = reader.GetDecimal(2),
                                MontoServiciosAdicionales = reader.GetDecimal(3),
                                ReservaId = reader.GetInt32(4),
                                TipoPagoId = reader.GetInt32(5),
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task<List<Factura>> GetByReservaId(int reservaId)
        {
            var facturas = new List<Factura>();
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT f.""ID_Factura"", f.""fecha_emision"", f.""monto_sala"", 
                               f.""monto_servicios_adicionales"", f.""RES_ID_Reserva"", 
                               f.""TPO_ID_Tipo_pago"", t.""nombre""
                        FROM ""Facturas"" f
                        JOIN ""Tipos_Pagos"" t ON f.""TPO_ID_Tipo_pago"" = t.""ID_Tipo_pago""
                        WHERE f.""RES_ID_Reserva"" = :reservaId";
                    command.Parameters.Add(new OracleParameter("reservaId", reservaId));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            facturas.Add(new Factura
                            {
                                Id = reader.GetInt32(0),
                                FechaEmision = reader.GetDateTime(1),
                                MontoSala = reader.GetDecimal(2),
                                MontoServiciosAdicionales = reader.GetDecimal(3),
                                ReservaId = reader.GetInt32(4),
                                TipoPagoId = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return facturas;
        }

        public async Task<int> CreateFactura(int reservaId, int tipoPagoId)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "crear_factura";

                    // Input parameters
                    command.Parameters.Add(new OracleParameter("p_reserva_id", OracleDbType.Int32, reservaId, ParameterDirection.Input));
                    command.Parameters.Add(new OracleParameter("p_tipo_pago_id", OracleDbType.Int32, tipoPagoId, ParameterDirection.Input));

                    // Output parameter for the ID
                    var idParam = new OracleParameter("p_id_factura", OracleDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(idParam);

                    await command.ExecuteNonQueryAsync();

                    return Convert.ToInt32(((OracleDecimal)idParam.Value).ToString());
                }
            }
        }
    }
}