using GeekShop.Domain;
using System.Data;
using Dapper;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using GeekShop.Domain.Exceptions;

namespace GeekShop.Repositories
{
    public class SqlProductRepository : IProductRepository
    {
        private readonly IDbContext _context;
        public SqlProductRepository(IDbContext context)
        {
            _context = context; 
        }
        public async Task<Product> Add(Product product)
        {
            var query = @"
                INSERT INTO Products
                (Title, Author, Description, Price)
                VALUES 
                (@Title, @Author, @Description, @Price)
                SELECT SCOPE_IDENTITY();";

            using (IDbConnection connection = _context.CreateConnection())
            {
                try
                {
                    product.Id = await connection.QuerySingleAsync<int>(query, new
                    {
                        Title = product.Title,
                        Author = product.Author,
                        Description = product.Description,
                        Price = product.Price
                    });
                    return (await Get(product.Id))!;
                }
                catch 
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
                
            }
        }

        public async Task Delete(int id)
        {
            var query = @"
                DELETE FROM Products
                WHERE Id = @Id";

            using (IDbConnection connection = _context.CreateConnection())
            {
                try
                {
                    await connection.QueryAsync(query, new { Id = id});
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
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
                try
                {
                    return await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id });
                }
                catch 
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }               
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
                try
                {
                    return await connection.QueryAsync<Product>(query, new { Ids = ids.ToArray() });
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
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
                try
                {
                    await connection.QueryAsync(query, new
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Author = product.Author,
                        Description = product.Description,
                        Price = product.Price
                    });
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }        
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
                try
                {
                    return await connection.QueryAsync<Product>(query);
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
            }
        }        
    }
}
