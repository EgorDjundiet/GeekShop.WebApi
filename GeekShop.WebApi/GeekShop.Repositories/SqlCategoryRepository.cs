using Dapper;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;

namespace GeekShop.Repositories
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly IDbContext _context;
        public SqlCategoryRepository(IDbContext context)
        {
            _context = context;
        }
        public async Task<int> Add(CategoryWithParent category)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
                
            var sql = @"
                INSERT INTO Categories
                (Name, Description)
                VALUES(@Name, @Description)
            
                DECLARE @CatIdVariable INT
                SELECT @CatIdVariable = SCOPE_IDENTITY();
            
                INSERT INTO CatHierarchy
                (CatId, ParentId)
                VALUES(@CatIdVariable, @ParentId)
            
                SELECT @CatIdVariable";

            var dynamicParam = new DynamicParameters();
            dynamicParam.Add("Name", category.Name);
            dynamicParam.Add("Description", category.Description);
            dynamicParam.Add("ParentId", category.ParentId);

            try
            {
                return await connection.QuerySingleAsync<int>(sql, dynamicParam, transaction: transaction);
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
                
            var sql = @"                   
                DECLARE @ParentId INT
                SELECT @ParentId = ParentId FROM CatHierarchy
                WHERE Id = @Id
		        
                UPDATE CatHierarchy
                SET ParentId = @ParentId
                WHERE ParentId = @Id
                
                DELETE FROM CatHierarchy WHERE CatId = @Id
                DELETE FROM Categories WHERE Id = @Id";

            try
            {
                await connection.QueryAsync(sql, new { Id = id }, transaction: transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
                                   
        }

        public async Task<CategoryWithParent?> Get(int id)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var sql = @"
                SELECT c.Id, c.Name, c.Description, h.Id, h.CatId, h.ParentId
                FROM Categories c
                JOIN CatHierarchy h ON c.Id = h.CatId
                WHERE c.Id = @Id";

            try
            {
                return (await connection.QueryAsync<CategoryWithParent, CatHierarchy, CategoryWithParent>(sql, (catWithParent, catHierarchy) =>
                {
                    catWithParent.ParentId = catHierarchy.ParentId;
                    return catWithParent;
                }, new { Id = id }, transaction:transaction)).SingleOrDefault();
            }
            catch
            {
                _context.Rollback();
                throw;
            } 
        }

        public async Task<IEnumerable<CategoryWithParent>> GetAll()
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var sql = @"
                SELECT c.Id, c.Name, c.Description, h.Id, h.CatId, h.ParentId
                FROM Categories c
                JOIN CatHierarchy h ON c.Id = h.CatId";

            try
            {
                return await connection.QueryAsync<CategoryWithParent, CatHierarchy, CategoryWithParent>(sql, (catWithParent, catHierarchy) =>
                {
                    catWithParent.ParentId = catHierarchy.ParentId;
                    return catWithParent;
                }, transaction:transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task Update(int id, CategoryWithParent category)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var sql = @"
                UPDATE Categories
                SET Name = @Name, Description = @Description
                WHERE Id = @Id
                
                UPDATE CatHierarchy
                SET ParentId = @ParentId
                WHERE CatId = @Id";
            
            var dynamicParam = new DynamicParameters();
            dynamicParam.Add("Id", id);
            dynamicParam.Add("Name", category.Name);
            dynamicParam.Add("Description", category.Description);
            dynamicParam.Add("ParentId", category.ParentId);
            
            try
            {
                await connection.QueryAsync<int>(sql, dynamicParam, transaction: transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }
    }
}
