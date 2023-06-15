using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;

namespace GeekShop.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
            Categories = new SqlCategoryRepository(_context);
            Customers = new SqlCustomerRepository(_context);
            Products = new SqlProductRepository(_context);
            Payments = new SqlPaymentRepository(_context);
            Orders = new SqlOrderRepository(_context);
        }

        public ICategoryRepository Categories { get; }

        public ICustomerRepository Customers { get; }

        public IProductRepository Products { get; }

        public IPaymentRepository Payments { get; }

        public IOrderRepository Orders { get; }

        public void SaveChanges()
        {
            _context.Commit();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
