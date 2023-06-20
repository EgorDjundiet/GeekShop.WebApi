using System.Data;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.Settings;
using Microsoft.Data.SqlClient;

namespace GeekShop.Repositories.Contexts
{
    public interface IDbContext : IDisposable 
    {
        IDbConnection GetConnection();
        IDbTransaction GetTransaction();
        void Rollback();
    }

    public class DbContext : IDbContext
    {
        private readonly string _connectionString;
        private readonly IDbTransaction _transaction;
        private readonly IDbConnection _connection;
        private bool _rolledback = false;
        public DbContext(IDbSettings dbSettings)
        {
            _connectionString = dbSettings.ConnectionString!;
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();         
        }
        public IDbConnection GetConnection() => _connection;
        public IDbTransaction GetTransaction() => _transaction;
        public void Rollback()
        {
            _transaction.Rollback();
            _rolledback = true;
        }       
        public void Dispose()
        {
            if (!_rolledback)
            {
                _transaction.Commit();
            }
            _connection.Close();
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
