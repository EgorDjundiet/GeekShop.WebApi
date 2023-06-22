using GeekShop.Domain;
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
        public async Task<int> Add(Product product)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
                
            var query = @"
                INSERT INTO Products
                (Title, Author, Description, Price, CategoryName)
                VALUES 
                (@Title, @Author, @Description, @Price, @CategoryName)
                SELECT SCOPE_IDENTITY();";
            try
            {
                product.Id = await connection.QuerySingleAsync<int>(query, new
                {
                    Title = product.Title,
                    Author = product.Author,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryName = product.CategoryName
                }, transaction: transaction);
                return product.Id;
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task Delete(int id)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var query = @"
                DELETE FROM Products
                WHERE Id = @Id";
            try
            {
                await connection.QueryAsync(query, new { Id = id }, transaction: transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task<Product?> Get(int id)
        {
            var query = @"
                SELECT Id, Title, Author, Description, Price, CategoryName
                FROM Products
                WHERE Id = @Id";

            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
            
            try
            {
                return await connection.QueryFirstOrDefaultAsync<Product>(query, new { Id = id }, transaction:transaction);
            }
            catch 
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetByIds(IEnumerable<int> ids)
        {
            var query = @"
                SELECT Id, Title, Author, Description, Price, CategoryName
                FROM Products
                WHERE Id IN @Ids";
    
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            try
            {
                return await connection.QueryAsync<Product>(query, new { Ids = ids.ToArray() }, transaction:transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task Update(Product product)
        {
            var query = @"
                UPDATE Products
                SET Title = @Title, Author = @Author, Description = @Description, Price = @Price, CategoryName = @CategoryName
                WHERE Id = @Id";

            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
                    
            try
            {
                await connection.QueryAsync(query, new
                {
                    Id = product.Id,
                    Title = product.Title,
                    Author = product.Author,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryName = product.CategoryName
                }, transaction:transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var query = @"
                SELECT Id, Title, Author, Description, Price, CategoryName
                FROM Products";

            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            try
            {
                return await connection.QueryAsync<Product>(query,transaction:transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }        
    }
}
