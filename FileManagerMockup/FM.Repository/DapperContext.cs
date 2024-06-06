using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FM.Repository
{
    public class DapperContext
    {
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
