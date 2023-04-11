using GeekShop.Domain;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace GeekShop.Repositories
{
    public class SqlProductRepository : IProductRepository
    {
        const string _connectionString = "Server=.\\SQLEXPRESS;Initial Catalog=GeekShop;Integrated Security=True;TrustServerCertificate=True";

        public async Task Add(Product product)
        {
            var query = @"
                INSERT INTO Products
                ( TITLE, AUTHOR,""DESCRIPTION"")
                Values 
                ( @Title, @Author, @Description)";

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<Product>(query, new {Title = product.Title, Author = product.Author, Description = product.Description });
            }
        }

        public async Task Delete(int id)
        {
            var query = @"
                DELETE FROM Products
                WHERE Id = @Id";

            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<Product>(query, new { Id = id});
            }
        }

        public async Task<Product?> Get(int id)
        {
            var query = @"
                SELECT 
                ID as Id,
                TITLE as Title,
                AUTHOR as Author,
                DESCRIPTION as Description
                FROM Products
                WHERE Id = @Id";

            using (IDbConnection connection = new SqlConnection(_connectionString))
            { 
                var resultEnumerable = await connection.QueryAsync<Product>(query, new { Id = id});
                return resultEnumerable.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var query = @"
                SELECT 
                ID as Id,
                TITLE as Title,
                AUTHOR as Author,
                DESCRIPTION as Description
                FROM Products";


            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Product>(query);
            }
        }
    }
}
