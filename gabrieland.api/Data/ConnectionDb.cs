namespace gabrieland.api.Data
{
    abstract public class ConnectionDb
    {
        protected readonly string _connectionString;
        public ConnectionDb(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleConnection");
        }
    }
}
