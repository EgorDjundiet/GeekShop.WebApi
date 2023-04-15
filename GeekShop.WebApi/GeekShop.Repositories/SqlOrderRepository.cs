using Dapper;
using GeekShop.Domain;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Linq;


namespace GeekShop.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        const string _connectionString = "Server=.\\SQLEXPRESS;Initial Catalog=GeekShop;Integrated Security=True;TrustServerCertificate=True";
        private readonly IProductRepository _productRepository;
        public SqlOrderRepository(IProductRepository repository)
        {
            _productRepository = repository;
        }
        public async Task Add(Order order)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                order.Date = DateTime.UtcNow;
                var orderId = (await connection.QueryAsync<int>(@"
                INSERT INTO Orders (CustomerName, CustomerAddress, Date, PhoneNumber)
                VALUES (@CustomerName, @CustomerAddress, @Date, @PhoneNumber);
                SELECT SCOPE_IDENTITY()", order)).Single();

                foreach (var orderDetail in order.Details)
                {
                    await connection.QueryAsync(@"
                    INSERT INTO OrderDetails (OrderId, ProductId, Quantity)
                    VALUES (@OrderId, @ProductId, @Quantity);", new
                    {
                        OrderId = orderId,
                        ProductId = orderDetail.ProductId,
                        Quantity = orderDetail.Quantity
                    });
                }
            }    
        }

        public async Task Delete(int id)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync(@"DELETE FROM Orders WHERE Id = @Id
                                               DELETE FROM OrderDetails WHERE OrderId = @Id", new { Id = id });
            }
                
        }

        public async Task<Order?> Get(int id)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var orderDictionary = new Dictionary<int, Order>();

                var sql = @"
                SELECT *
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

                    if (orderDetail != null)
                    {
                        orderDetail.Product = product;
                        currentOrder.Details.Add(orderDetail);
                    }

                    return currentOrder;
                }, new { Id = id }, splitOn: "Id,OrderId,Id");

                return orderDictionary.Values.SingleOrDefault();
            } 
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                var orderDictionary = new Dictionary<int, Order>();

                var sql = @"
                SELECT *
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
                }, splitOn: "Id,OrderId,Id");

                return orderDictionary.Values;
            }
        }

        public async Task Update(Order order)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                await connection.QueryAsync<int>(@"
                    UPDATE Orders
                    SET CustomerName = @CustomerName, 
                        CustomerAddress = @CustomerAddress, 
                        PhoneNumber = @PhoneNumber
                    WHERE Id = @Id", order);

                foreach (var orderDetail in order.Details)
                {
                    await connection.QueryAsync(@"
                    UPDATE OrderDetails
                    SET ProductId = @ProductId, Quantity = @Quantity", new
                    {
                        ProductId = orderDetail.ProductId,
                        Quantity = orderDetail.Quantity
                    });
                }
            }
        }
    }
}

