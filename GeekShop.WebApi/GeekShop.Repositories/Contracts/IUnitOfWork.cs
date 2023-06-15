namespace GeekShop.Repositories.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        ICustomerRepository Customers { get; }
        IProductRepository Products { get; }
        IPaymentRepository Payments { get; }
        IOrderRepository Orders { get; }

        void SaveChanges();
    }
}
