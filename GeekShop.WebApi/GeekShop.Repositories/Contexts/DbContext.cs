using System.Data;
using GeekShop.Domain.Settings;
using Microsoft.Data.SqlClient;

namespace GeekShop.Repositories.Contexts
{
    public interface IDbContext 
    {
        IDbConnection CreateConnection();
    }

    public class DbContext : IDbContext
    {
        private readonly string _connectionString;

        public DbContext(IDbSettings dbSettings)
        {
            _connectionString = dbSettings.ConnectionString!;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
