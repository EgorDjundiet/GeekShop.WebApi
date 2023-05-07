using Dapper;
using GeekShop.Domain;
using System.Data;
using GeekShop.Repositories.Contracts;
using GeekShop.Repositories.Contexts;
using GeekShop.Domain.ViewModels;

namespace GeekShop.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly IDbContext _context;
        private readonly IProductRepository _productRepository;
        
        public SqlOrderRepository(IDbContext context, IProductRepository repository)
        {
            _context = context;
            _productRepository = repository;
        }
        public async Task Add(Order order)
        {
            var sqlOrder = @"
                INSERT INTO Orders (CustomerName, CustomerAddress, Date, PhoneNumber, Email)
                VALUES (@CustomerName, @CustomerAddress, @Date, @PhoneNumber, @Email);
                SELECT SCOPE_IDENTITY()";

            var sqlOrderDetails = @"
                INSERT INTO OrderDetails(OrderId, ProductId, ProductQuantity)
                VALUES(@OrderId, @ProductId, @ProductQuantity);";

            using (IDbConnection connection = _context.CreateConnection())
            {               
                var orderId = await connection.QuerySingleAsync<int>(sqlOrder, new 
                {
                    CustomerName = order.CustomerName,
                    CustomerAddress = order.CustomerAddress,
                    PhoneNumber = order.PhoneNumber,
                    Email = order.Email,
                    Date = DateTime.UtcNow
                });

                foreach (var orderDetail in order.Details)
                {
                    await connection.QueryAsync(sqlOrderDetails, new
                    {
                        OrderId = orderId,
                        ProductId = orderDetail.Product.Id,
                        ProductQuantity = orderDetail.ProductQuantity
                    });
                }
            }    
        }

        public async Task Delete(int id)
        {
            var sql = @"DELETE FROM OrderDetails WHERE OrderId = @Id
                        DELETE FROM Orders WHERE Id = @Id";

            using (IDbConnection connection = _context.CreateConnection())
            {
                await connection.QueryAsync(sql, new { Id = id });
            }
                
        }

        public async Task<Order?> Get(int id)
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var orderDictionary = new Dictionary<int, Order>();

                var sql = @"
                SELECT o.Id, o.CustomerName, o.CustomerAddress, o.PhoneNumber, o.Email, o.Date, od.Id, od.ProductQuantity, p.Id, p.Title, p.Author, p.Description, p.Price
                FROM Orders o
                LEFT JOIN OrderDetails od ON o.Id = od.OrderId
                LEFT JOIN Products p ON od.ProductId = p.Id
                WHERE o.Id = @Id";

                await connection.QueryAsync<Order, OrderDetails, Product, Order>(sql, (order, orderDetail, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Details = new List<OrderDetails>();
                        orderDictionary.Add(currentOrder.Id, currentOrder);
                    }

                    if (orderDetail is not null)
                    {
                        orderDetail.Product = product;
                        currentOrder.Details.Add(orderDetail);
                    }

                    return currentOrder;
                }, new { Id = id });

                return orderDictionary.Values.SingleOrDefault();
            } 
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var orderDictionary = new Dictionary<int, Order>();

                var sql = @"
                SELECT o.Id, o.CustomerName, o.CustomerAddress, o.PhoneNumber, o.Email, o.Date, od.Id, od.ProductQuantity, p.Id, p.Title, p.Author, p.Description, p.Price
                FROM Orders o
                LEFT JOIN OrderDetails od ON o.Id = od.OrderId
                LEFT JOIN Products p ON od.ProductId = p.Id";

                await connection.QueryAsync<Order, OrderDetails, Product, Order>(sql, (order, orderDetail, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Details = new List<OrderDetails>();
                        orderDictionary.Add(currentOrder.Id, currentOrder);
                    }

                    if (orderDetail != null)
                    {
                        orderDetail.Product = product;
                        currentOrder.Details.Add(orderDetail);
                    }

                    return currentOrder;
                });

                return orderDictionary.Values;
            }
        }

        public async Task<IEnumerable<Order>> GetByIds(IEnumerable<int> ids)
        {
            using (IDbConnection connection = _context.CreateConnection())
            {
                var orderDictionary = new Dictionary<int, Order>();

                var sql = @"
                SELECT o.Id, o.CustomerName, o.CustomerAddress, o.PhoneNumber, o.Email, o.Date, od.Id, od.ProductQuantity, p.Id, p.Title, p.Author, p.Description, p.Price
                FROM Orders o
                LEFT JOIN OrderDetails od ON o.Id = od.OrderId
                LEFT JOIN Products p ON od.ProductId = p.Id
                WHERE o.Id in @Ids";

                await connection.QueryAsync<Order, OrderDetails, Product, Order>(sql, (order, orderDetail, product) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.Details = new List<OrderDetails>();
                        orderDictionary.Add(currentOrder.Id, currentOrder);
                    }

                    if (orderDetail is not null)
                    {
                        orderDetail.Product = product;
                        currentOrder.Details.Add(orderDetail);
                    }

                    return currentOrder;
                }, new { Ids = ids.ToArray() });

                return orderDictionary.Values;
            }
        }

        public async Task Update(Order order)
        {
            var sqlOrder = @"
                UPDATE Orders
                SET CustomerName = @CustomerName, CustomerAddress = @CustomerAddress, PhoneNumber = @PhoneNumber, Email = @Email
                WHERE Id = @Id";

            var sqlOrderDetails = @"
                UPDATE OrderDetails
                SET ProductId = @ProductId, ProductQuantity = @ProductQuantity";

            using (IDbConnection connection = _context.CreateConnection())
            {               
                await connection.QueryAsync(sqlOrder, order);

                foreach (var orderDetail in order.Details)
                {
                    await connection.QueryAsync(sqlOrderDetails, new
                    {
                        ProductId = orderDetail.Product.Id,
                        ProductQuantity = orderDetail.ProductQuantity
                    });
                }
            }
        }
    }
}

