using Dapper;
using GeekShop.Domain;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeekShop.Repositories
{
    public class SqlCustomerRepository : ICustomerRepository
    {
        const string _connectionString = "Server=.\\SQLEXPRESS;Initial Catalog=GeekShop;Integrated Security=True;TrustServerCertificate=True";
        public async Task Add(Customer customer)
        {
            using(IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<Customer>(@"
                    INSERT INTO Customers
                    (Name, Address, PhoneNumber, Email)
                    VALUES(@Name,@Address,@PhoneNumber,@Email)", customer);
            }
        }

        public async Task Delete(int id)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<Customer>(@"
                    DELETE FROM Customers
                    WHERE Id = @Id", new {Id = id});
            }
        }

        public async Task<Customer?> Get(int id)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return (await connection.QueryAsync<Customer>(@"
                    SELECT Id, Name, Address, PhoneNumber, Email FROM Customers
                    WHERE Id = @Id", new {Id = id})).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Customer>(@"
                    SELECT Id, Name, Address, PhoneNumber, Email FROM Customers");
            }
        }

        public async Task Update(Customer customer)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<Customer>(@"
                    UPDATE Customers
                    SET Name = @Name, Address = @Address, PhoneNumber = @PhoneNumber, Email = @Email
                    WHERE Id = @Id", customer);
            }
        }
    }
}
