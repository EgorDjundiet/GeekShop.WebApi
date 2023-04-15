//using GeekShop.Domain;
//using System.Data;

//public class SqlOrderRepository : IOrderRepository
//{
//    private readonly IDbConnection _connection;

//    public SqlOrderRepository(IDbConnection connection)
//    {
//        _connection = connection;
//    }

//    public async Task Add(Order order)
//    {
//        var orderId = _connection.Query<int>(@"
//            INSERT INTO Orders (CustomerName, CustomerPhone, Date)
//            VALUES (@CustomerName, @CustomerPhone, @Date);
//            SELECT LAST_INSERT_ID();", order).Single();

//        foreach (var orderDetail in order.Details)
//        {
//            _connection.Execute(@"
//                INSERT INTO OrderDetails (OrderId, ProductId, Quantity)
//                VALUES (@OrderId, @ProductId, @Quantity);", new
//            {
//                OrderId = orderId,
//                ProductId = orderDetail.Product.Id,
//                Quantity = orderDetail.Quantity
//            });
//        }
//    }

//    public Order GetById(int id)
//    {
//        var orderDictionary = new Dictionary<int, Order>();

//        var sql = @"
//            SELECT *
//            FROM Orders o
//            LEFT JOIN OrderDetails od ON o.Id = od.OrderId
//            LEFT JOIN Products p ON od.ProductId = p.Id
//            WHERE o.Id = @Id";

//        _connection.Query<Order, OrderDetails, Product, Order>(sql, (order, orderDetail, product) =>
//        {
//            if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
//            {
//                currentOrder = order;
//                currentOrder.OrderDetails = new List<OrderDetails>();
//                orderDictionary.Add(currentOrder.Id, currentOrder);
//            }

//            if (orderDetail != null)
//            {
//                orderDetail.Product = product;
//                currentOrder.OrderDetails.Add(orderDetail);
//            }

//            return currentOrder;
//        }, new { Id = id });

//        return orderDictionary.Values.SingleOrDefault();
//    }

//    public void Update(Order order)
//    {
//        _connection.Execute(@"
//            UPDATE Orders
//            SET CustomerName = @CustomerName, CustomerPhone = @CustomerPhone, Date = @Date
//            WHERE Id = @Id;", order);

//        foreach (var orderDetail in order.Details)
//        {
//            _connection.Execute(@"
//                UPDATE OrderDetails
//                SET ProductId = @ProductId, Quantity = @Quantity
//                WHERE Id = @Id;", new
//            {
//                Id = orderDetail.Id,
//                ProductId = orderDetail.Product.Id,
//                Quantity = orderDetail.Quantity
//            });
//        }
//    }

//    public void Remove(Order order)
//    {
//        _connection.Execute(@"
//            DELETE FROM OrderDetails WHERE OrderId = @Id;
//            DELETE FROM Orders WHERE Id = @Id;", order);
//    }

//    public IEnumerable<Order> GetAll()
//    {
//        var orderDictionary = new Dictionary<int, Order>();

//        var sql = @"
//            SELECT *
//            FROM Orders o
//            LEFT JOIN OrderDetails od ON o.Id = od.OrderId
//            LEFT JOIN Products p ON od.ProductId = p.Id";

//        _connection.Query<Order, OrderDetails, Product, Order>(sql, (order, orderDetail, product) =>
//        {
//            if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
//            {
//                currentOrder = order;
//                currentOrder.OrderDetails = new List<OrderDetails>();
//                orderDictionary.Add(currentOrder.Id, currentOrder);
//            }

//            if (orderDetail != null)
//            {
//                orderDetail.Product = product;
//                currentOrder.OrderDetails.Add(orderDetail);
//            }

//            return currentOrder;
//        });

//        return orderDictionary.Values;
//    }
//}