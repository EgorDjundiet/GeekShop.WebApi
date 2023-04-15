using GeekShop.Domain;

public interface IOrderRepository
{
    Task Add(Order order);
    Task<IEnumerable<Order>> GetAll();
    Task<Order?> Get(int id);
    Task Delete(int id);
    Task Update(Order order);
}

