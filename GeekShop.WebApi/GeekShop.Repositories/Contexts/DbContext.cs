using System.Data;
using GeekShop.Domain.Settings;
using Microsoft.Data.SqlClient;

namespace GeekShop.Repositories.Contexts
{
    public interface IDbContext : IDisposable 
    {
        IDbConnection GetConnection();
        IDbTransaction GetTransaction();
        void Commit();
        void Rollback();
    }

    public class DbContext : IDbContext
    {
        private readonly string _connectionString;
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        public DbContext(IDbSettings dbSettings)
        {
            _connectionString = dbSettings.ConnectionString!;
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();         
        }
        public IDbConnection GetConnection() => _connection;
        public IDbTransaction GetTransaction() => _transaction;
        public void Commit()
        {
            _transaction.Commit();
        }
        public void Rollback()
        {
            _transaction.Rollback();
        }       
        public void Dispose()
        {
            _connection.Close();
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
