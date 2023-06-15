using Dapper;
using GeekShop.Domain;
using GeekShop.Repositories.Contracts;
using GeekShop.Repositories.Contexts;
using GeekShop.Domain.Exceptions;

namespace GeekShop.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly IDbContext _context;
        
        public SqlOrderRepository(IDbContext context)
        {
            _context = context;
        }
        public async Task<int> Add(Order order)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
                
            var sqlOrder = @"
                INSERT INTO Orders (CustomerName, CustomerAddressId, Date, PhoneNumber, Email, Status)
                VALUES (@CustomerName, @CustomerAddressId, @Date, @PhoneNumber, @Email, @Status);
                SELECT SCOPE_IDENTITY()";
                
            var sqlOrderDetails = @"
                INSERT INTO OrderDetails(OrderId, ProductTitle, ProductAuthor, ProductDescription, ProductPrice, ProductQuantity, ProductCategoryName)
                VALUES(@OrderId, @ProductTitle, @ProductAuthor, @ProductDescription, @ProductPrice, @ProductQuantity, @ProductCategoryName);";
                
            var sqlAddress = @"
                INSERT INTO Addresses
                (Street, City, State, ZipCode, Country)
                VALUES(@Street, @City, @State, @ZipCode, @Country)
                SELECT SCOPE_IDENTITY()";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Street",order.CustomerAddress.Street);
                parameters.Add("City",order.CustomerAddress.City);
                parameters.Add("State",order.CustomerAddress.State);
                parameters.Add("ZipCode",order.CustomerAddress.ZipCode);
                parameters.Add("Country",order.CustomerAddress.Country);
                var addressId = await connection.QuerySingleAsync<int>(sqlAddress, parameters, transaction:transaction);
            
                parameters.Add("CustomerName", order.CustomerName);
                parameters.Add("CustomerAddressId", addressId);
                parameters.Add("PhoneNumber", order.PhoneNumber);
                parameters.Add("Email", order.Email);
                parameters.Add("Date", order.Date);
                parameters.Add("Status", order.Status.ToString());
                var orderId = await connection.QuerySingleAsync<int>(sqlOrder, parameters, transaction:transaction);
            
                foreach (var orderDetail in order.Details)
                {
                    parameters.Add("OrderId", orderId);
                    parameters.Add("ProductTitle", orderDetail.ProductTitle);
                    parameters.Add("ProductAuthor", orderDetail.ProductAuthor);
                    parameters.Add("ProductDescription", orderDetail.ProductDescription);
                    parameters.Add("ProductPrice", orderDetail.ProductPrice);
                    parameters.Add("ProductQuantity", orderDetail.ProductQuantity);
                    parameters.Add("ProductCategoryName",orderDetail.ProductCategoryName);
                    await connection.QueryAsync(sqlOrderDetails, parameters, transaction:transaction);
                }
            
                return orderId;
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

            var order = await Get(id);
            var sql = @"
                DELETE FROM OrderDetails WHERE OrderId = @Id
                DELETE FROM Orders WHERE Id = @Id
                DELETE FROM Addresses WHERE Id = @a_Id";

            try
            {
                await connection.QueryAsync(sql, new { Id = id, a_Id = order!.CustomerAddress.Id }, transaction:transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }      
        }

        public async Task<Order?> Get(int id)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var orderDictionary = new Dictionary<int, Order>();
            
            var sql = @"
            SELECT o.Id, o.CustomerName, o.PhoneNumber, o.Email, o.Date, o.Status, 
                   a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country, 
                   od.Id, od.ProductTitle, od.ProductAuthor, od.ProductDescription, od.ProductPrice, od.ProductQuantity, od.ProductCategoryName
            FROM Orders o
            LEFT JOIN Addresses a ON o.CustomerAddressId = a.Id
            LEFT JOIN OrderDetails od ON o.Id = od.OrderId
            WHERE o.Id = @Id";
            
            try
            {
                await connection.QueryAsync<Order, Address, OrderDetails, Order>(sql, (order, address, orderDetail) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.CustomerAddress = address;
                        currentOrder.Details = new List<OrderDetails>();
                        orderDictionary.Add(currentOrder.Id, currentOrder);
                    }
            
                    if (orderDetail is not null)
                    {
                        currentOrder.Details.Add(orderDetail);
                    }
            
                    return currentOrder;
                }, new { Id = id }, transaction:transaction);
                return orderDictionary.Values.SingleOrDefault();
            }
            catch 
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
            
            var orderDictionary = new Dictionary<int, Order>();
            
            var sql = @"
            SELECT o.Id, o.CustomerName, o.PhoneNumber, o.Email, o.Date, o.Status, 
                   a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country, 
                   od.Id, od.ProductTitle, od.ProductAuthor, od.ProductDescription, od.ProductPrice, od.ProductQuantity, od.ProductCategoryName
            FROM Orders o
            LEFT JOIN Addresses a ON o.CustomerAddressId = a.Id
            LEFT JOIN OrderDetails od ON o.Id = od.OrderId";
            
            try
            {
                await connection.QueryAsync<Order, Address, OrderDetails, Order>(sql, (order, address, orderDetail) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.CustomerAddress = address;
                        currentOrder.Details = new List<OrderDetails>();
                        orderDictionary.Add(currentOrder.Id, currentOrder);
                    }
            
                    if (orderDetail is not null)
                    {
                        currentOrder.Details.Add(orderDetail);
                    }
            
                    return currentOrder;
                }, transaction:transaction);
            
                return orderDictionary.Values;
            }
            catch 
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetByIds(IEnumerable<int> ids)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var orderDictionary = new Dictionary<int, Order>();
            
            var sql = @"
            SELECT o.Id, o.CustomerName, o.PhoneNumber, o.Email, o.Date, o.Status, 
                   a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country, 
                   od.Id, od.ProductTitle, od.ProductAuthor, od.ProductDescription, od.ProductPrice, od.ProductQuantity, od.ProductCategoryName
            FROM Orders o
            LEFT JOIN Addresses a ON o.CustomerAddressId = a.Id
            LEFT JOIN OrderDetails od ON o.Id = od.OrderId
            WHERE o.Id in @Ids";
            
            try
            {
                await connection.QueryAsync<Order, Address, OrderDetails, Order>(sql, (order, address, orderDetail) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var currentOrder))
                    {
                        currentOrder = order;
                        currentOrder.CustomerAddress = address;
                        currentOrder.Details = new List<OrderDetails>();
                        orderDictionary.Add(currentOrder.Id, currentOrder);
                    }
            
                    if (orderDetail is not null)
                    {
                        currentOrder.Details.Add(orderDetail);
                    }
            
                    return currentOrder;
                }, new { Ids = ids }, transaction:transaction);
            
                return orderDictionary.Values;
            }
            catch
            {
                _context.Rollback();
                throw;
            } 
        }        

        public async Task Update(Order order)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
                
            var sqlOrder = @"
                UPDATE Orders
                SET CustomerName = @CustomerName, PhoneNumber = @PhoneNumber, Email = @Email
                WHERE Id = @Id
                SELECT CustomerAddressId FROM Orders WHERE Id = @Id";
            
            var sqlOrderDetails = @"
                UPDATE OrderDetails
                SET ProductTitle = @ProductTitle, ProductAuthor = @ProductAuthor, 
                    ProductDescription = @ProductDescription, ProductPrice = @ProductPrice, ProductQuantity = @ProductQuantity, ProductCategoryName = @ProductCategoryName
                WHERE OrderId = @Id";
            
            var sqlAddress = @"
                UPDATE Addresses
                SET Street = @Street, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country
                WHERE Id = @Id";
            
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", order.Id);
                parameters.Add("CustomerName", order.CustomerName);
                parameters.Add("PhoneNumber", order.PhoneNumber);
                parameters.Add("Email", order.Email);
            
                var addressId = await connection.QuerySingleAsync<int>(sqlOrder, parameters, transaction: transaction);
            
                parameters.Add("Id", addressId);
                parameters.Add("Street", order.CustomerAddress.Street);
                parameters.Add("State", order.CustomerAddress.State);
                parameters.Add("City", order.CustomerAddress.City);
                parameters.Add("ZipCode", order.CustomerAddress.ZipCode);
                parameters.Add("Country", order.CustomerAddress.Country);
            
                await connection.QueryAsync(sqlAddress, parameters, transaction: transaction);
            
                foreach (var orderDetail in order.Details)
                {
                    parameters.Add("Id", order.Id);
                    parameters.Add("ProductTitle", orderDetail.ProductTitle);
                    parameters.Add("ProductAuthor", orderDetail.ProductAuthor);
                    parameters.Add("ProductDescription", orderDetail.ProductDescription);
                    parameters.Add("ProductPrice", orderDetail.ProductPrice);
                    parameters.Add("ProductQuantity", orderDetail.ProductQuantity);
                    parameters.Add("ProductCategoryName", orderDetail.ProductCategoryName);
            
                    await connection.QueryAsync(sqlOrderDetails, parameters, transaction: transaction);
                }
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }
        public async Task ChangeStatus(int id, OrderStatus status)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var sql = @"UPDATE Orders SET Status = @Status WHERE Id = @Id";

            try
            {
                await connection.QueryAsync(sql, new {Id = id, Status = status}, transaction: transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }
    }
}

