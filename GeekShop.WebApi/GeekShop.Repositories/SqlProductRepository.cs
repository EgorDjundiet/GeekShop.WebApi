using GeekShop.Domain;
using System.Data;
using Dapper;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;

namespace GeekShop.Repositories
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly Context _context;
        public SqlProductRepository(Context context)
        {
            _context = context; 
        }
        public async Task Add(Product product)
        {
            var query = @"
                INSERT INTO Products
                (Title, Author, Description, Price)
                Values 
                (@Title, @Author, @Description, @Price)";

            using (IDbConnection connection = _context.CreateConnection())
            {
                await connection.QueryAsync(query,new {Title = product.Title, Author = product.Author, Description = product.Description, Price = product.Price});
            }
        }

        public async Task Delete(int id)
        {
            var query = @"
                DELETE FROM Products
                WHERE Id = @Id";

            using (IDbConnection connection = _context.CreateConnection())
            {
                await connection.QueryAsync(query, new { Id = id});
            }
        }

        public async Task<Product?> Get(int id)
        {
            var query = @"
                SELECT 
                Id, 
                Title,
                Author,
                Description,
                Price
                FROM Products
                WHERE Id = @Id";

            using (IDbConnection connection = _context.CreateConnection())
            { 
                return await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id});
            }
        }

        public async Task<IEnumerable<Product>> GetByIds(IEnumerable<int> ids)
        {
            var query = @"
                SELECT 
                Id,
                Title,
                Author,
                Description,
                Price
                FROM Products
                WHERE Id IN @Ids";
    
            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(query, new { Ids = ids.ToArray() });               
            }

        }
        public async Task Update(Product product)
        {
            var query = @"
                UPDATE Products
                SET Title = @Title, Author = @Author, Description = @Description, Price = @Price
                WHERE Id = @Id";

            using (IDbConnection connection = _context.CreateConnection())
            {
                await connection.QueryAsync(query, new { 
                    Id = product.Id, 
                    Title = product.Title, 
                    Author = product.Author, 
                    Description = product.Description, 
                    Price = product.Price
                });
            }
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var query = @"
                SELECT 
                Id,
                Title,
                Author,
                Description,
                Price
                FROM Products";


            using (IDbConnection connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Product>(query);
            }
        }        
    }
}
