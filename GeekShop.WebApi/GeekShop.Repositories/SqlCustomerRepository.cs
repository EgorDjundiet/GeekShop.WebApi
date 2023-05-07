using Dapper;
using GeekShop.Domain;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GeekShop.Repositories
{
    public class SqlCustomerRepository : ICustomerRepository
    {
        private readonly IDbContext _context;
        public SqlCustomerRepository(IDbContext context)
        {
            _context = context;
        }
        public async Task Add(Customer customer)
        {
            var sql = @"
                INSERT INTO Customers
                (Name, Address, PhoneNumber, Email)
                VALUES(@Name,@Address,@PhoneNumber,@Email)";

            using(IDbConnection connection = _context.CreateConnection())
            {
                await connection.QueryAsync(sql, customer);
            }
        }

        public async Task Delete(int id)
        {
            var sql = @"
                DELETE FROM Customers
                WHERE Id = @Id";
            using (IDbConnection connection = _context.CreateConnection())
            {
                await connection.QueryAsync(sql, new {Id = id});
            }
        }

        public async Task<Customer?> Get(int id)
        {
            var sql = @"
                SELECT Id, Name, Address, PhoneNumber, Email FROM Customers
                WHERE Id = @Id";
            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Customer>(sql, new {Id = id});
            }
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            var sql = @"SELECT Id, Name, Address, PhoneNumber, Email FROM Customers";
            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Customer>(sql);
            }
        }

        public async Task<IEnumerable<Customer>> GetByIds(IEnumerable<int> ids)
        {
            var sql = @"
                SELECT Id,Name,Address,PhoneNumber,Email FROM Customers
                WHERE Id in @Ids";

            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Customer>(sql, new {Ids = ids.ToArray()});
            }
        }

        public async Task Update(Customer customer)
        {
            var sql = @"
                UPDATE Customers
                SET Name = @Name, Address = @Address, PhoneNumber = @PhoneNumber, Email = @Email
                WHERE Id = @Id";

            using (IDbConnection connection = _context.CreateConnection())
            {
                await connection.QueryAsync(sql, customer);
            }
        }
    }
}
