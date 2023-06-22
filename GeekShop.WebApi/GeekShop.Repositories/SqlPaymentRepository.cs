using Dapper;
using GeekShop.Domain;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;

namespace GeekShop.Repositories
{
    public class SqlPaymentRepository : IPaymentRepository
    {
        private readonly IDbContext _context;
        public SqlPaymentRepository(IDbContext context) 
        {
            _context = context;
        }
        public async Task<Payment?> Get(int id)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var sql = @"
                SELECT p.Id, p.Status, p.Amount, p.OrderId, p.AccountNumber,
                       a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country
                FROM Payments as p
                LEFT JOIN Addresses as a on p.BillingAddressId = a.Id
                WHERE p.Id = @Id
            ";

            try
            {
                return (await connection.QueryAsync<Payment, Address, Payment>(sql, (payment, address) =>
                {
                    payment.BillingAddress = address;
                    return payment;
                }, new { Id = id }, transaction:transaction)).SingleOrDefault();
            }
            catch 
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
            
            var sql = @"
                SELECT p.Id, p.Status, p.Amount, p.OrderId, p.AccountNumber,
                       a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country
                FROM Payments as p
                LEFT JOIN Addresses as a on p.BillingAddressId = a.Id
            ";

            try
            {
                return (await connection.QueryAsync<Payment, Address, Payment>(sql, (payment, address) =>
                {
                    payment.BillingAddress = address;
                    return payment;
                },transaction:transaction));
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
            
            var sql = @"
                SELECT p.Id, p.Status, p.Amount, p.OrderId, p.AccountNumber,
                       a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country
                FROM Payments as p
                LEFT JOIN Addresses as a on p.BillingAddressId = a.Id
                WHERE p.Id IN @Ids
            ";

            try
            {
                return (await connection.QueryAsync<Payment, Address, Payment>(sql, (payment, address) =>
                {
                    payment.BillingAddress = address;
                    return payment;
                }, new { Ids = ids.ToArray()}, transaction:transaction));
            }
            catch
            {
                _context.Rollback();
                throw;
            }                  
        }

        public async Task<int> Add(Payment payment, Order order)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
                
            var sqlPayment = @"
                INSERT INTO Addresses
                (Street, City, State, ZipCode, Country)
                VALUES(@Street, @City, @State, @ZipCode, @Country)
                
                DECLARE @AddressIdVariable INT 
                SELECT @AddressIdVariable = SCOPE_IDENTITY()
            
                INSERT INTO Payments
                (AccountNumber, OrderId, BillingAddressId, Status, Amount)
                VALUES(@AccountNumber, @OrderId, @AddressIdVariable, @Status, @Amount)
                SELECT SCOPE_IDENTITY();";

            var sqlOrder = @"
                INSERT INTO Addresses
                (Street, City, State, ZipCode, Country)
                VALUES(@Street, @City, @State, @ZipCode, @Country)
                
                DECLARE @AddressIdVariable INT 
                SELECT @AddressIdVariable = SCOPE_IDENTITY()

                INSERT INTO OrdersForPayments (PaymentId, CustomerName, CustomerAddressId, Date, PhoneNumber, Email, Status)
                VALUES (@PaymentId, @CustomerName, @AddressIdVariable, @Date, @PhoneNumber, @Email, @Status);
                SELECT SCOPE_IDENTITY();";

            var sqlOrderDetails = @"
                INSERT INTO OrderDetailsForPayments(OrderId, ProductTitle, ProductAuthor, ProductDescription, ProductPrice, ProductQuantity, ProductCategoryName)
                VALUES(@OrderId, @ProductTitle, @ProductAuthor, @ProductDescription, @ProductPrice, @ProductQuantity, @ProductCategoryName);";

            try 
            { 
                var parameters = new DynamicParameters();
                //Payment
                parameters.Add("Street", payment.BillingAddress.Street);
                parameters.Add("City", payment.BillingAddress.City);
                parameters.Add("State", payment.BillingAddress.State);
                parameters.Add("ZipCode", payment.BillingAddress.ZipCode);
                parameters.Add("Country", payment.BillingAddress.Country);
                parameters.Add("AccountNumber", payment.AccountNumber);
                parameters.Add("OrderId", payment.OrderId);
                parameters.Add("Status", payment.Status.ToString());
                parameters.Add("Amount", payment.Amount);
                var paymentId = await connection.QuerySingleAsync<int>(sqlPayment, parameters, transaction: transaction);
                //Order
                parameters.Add("PaymentId", paymentId);
                parameters.Add("Street", order.CustomerAddress.Street);
                parameters.Add("City", order.CustomerAddress.City);
                parameters.Add("State", order.CustomerAddress.State);
                parameters.Add("ZipCode", order.CustomerAddress.ZipCode);
                parameters.Add("Country", order.CustomerAddress.Country);
                parameters.Add("CustomerName", order.CustomerName);
                parameters.Add("PhoneNumber", order.PhoneNumber);
                parameters.Add("Email", order.Email);
                parameters.Add("Date", order.Date);
                parameters.Add("Status", order.Status.ToString());
                var orderId = await connection.QuerySingleAsync<int>(sqlOrder, parameters, transaction: transaction);

                foreach (var orderDetail in order.Details)
                {
                    parameters.Add("OrderId", orderId);
                    parameters.Add("ProductTitle", orderDetail.ProductTitle);
                    parameters.Add("ProductAuthor", orderDetail.ProductAuthor);
                    parameters.Add("ProductDescription", orderDetail.ProductDescription);
                    parameters.Add("ProductPrice", orderDetail.ProductPrice);
                    parameters.Add("ProductQuantity", orderDetail.ProductQuantity);
                    parameters.Add("ProductCategoryName", orderDetail.ProductCategoryName);
                    await connection.QueryAsync(sqlOrderDetails, parameters, transaction: transaction);
                }

                return paymentId;
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }

        public async Task Update(Payment payment, Order order)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();
            try 
            { 
                var sqlPayment = @"
                    UPDATE Payments
                    SET OrderId = @OrderId, Status = @Status, Amount = @Amount, AccountNumber = @AccountNumber
                    WHERE Id = @Id
                
                    DECLARE @AddressIdVariable INT 
                    SELECT @AddressIdVariable = BillingAddressId FROM Payments WHERE Id = @Id
                
                    UPDATE Addresses
                    SET Street = @Street, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country
                    WHERE Id = @AddressIdVariable";
                
                var sqlOrder = @"
                    UPDATE OrdersForPayments
                    SET CustomerName = @CustomerName, PhoneNumber = @PhoneNumber, Email = @Email
                    WHERE Id = @Id
                
                    DECLARE @AddressIdVariable INT 
                    SELECT @AddressIdVariable = CustomerAddressId FROM OrdersForPayments WHERE Id = @Id
                
                    UPDATE Addresses
                    SET Street = @Street, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country
                    WHERE Id = @AddressIdVariable";
                
                var sqlOrderDetails = @"
                    UPDATE OrderDetailsForPayments
                    SET ProductTitle = @ProductTitle, ProductAuthor = @ProductAuthor, 
                        ProductDescription = @ProductDescription, ProductPrice = @ProductPrice, 
                        ProductQuantity = @ProductQuantity, ProductCategoryName = @ProductCategoryName
                    WHERE OrderId = @Id";
                
                
                var parameters = new DynamicParameters();
                //Payment
                parameters.Add("Id", payment.Id);
                parameters.Add("OrderId", payment.OrderId);
                parameters.Add("Status", payment.Status.ToString());
                parameters.Add("Amount", payment.Amount);
                parameters.Add("AccountNumber", payment.AccountNumber);
                parameters.Add("Street", payment.BillingAddress.Street);
                parameters.Add("City", payment.BillingAddress.City);
                parameters.Add("State", payment.BillingAddress.State);
                parameters.Add("ZipCode", payment.BillingAddress.ZipCode);
                parameters.Add("Country", payment.BillingAddress.Country);
                await connection.QueryAsync(sqlPayment, parameters, transaction: transaction);
                //Order
                parameters.Add("Id", order.Id);
                parameters.Add("CustomerName", order.CustomerName);
                parameters.Add("PhoneNumber", order.PhoneNumber);
                parameters.Add("Email", order.Email);
                parameters.Add("Street", order.CustomerAddress.Street);
                parameters.Add("State", order.CustomerAddress.State);
                parameters.Add("City", order.CustomerAddress.City);
                parameters.Add("ZipCode", order.CustomerAddress.ZipCode);
                parameters.Add("Country", order.CustomerAddress.Country);
                await connection.QueryAsync(sqlOrder, parameters, transaction: transaction);

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

        public async Task Delete(int id)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var payment = await Get(id);
            var order = await GetOrderByPaymentId(id);
            var sql = @"
                DELETE FROM OrderDetailsForPayments WHERE OrderId = @o_Id
                DELETE FROM OrdersForPayments WHERE PaymentId = @p_id
                DELETE FROM Payments WHERE Id = @p_Id
                DELETE FROM Addresses WHERE Id = @a_Id";

            try
            {
                await connection.QueryAsync(sql, new { a_Id = payment!.BillingAddress.Id, p_Id = id, o_id = order!.Id}, transaction: transaction);
            }
            catch
            {
                _context.Rollback();
                throw;
            }      
        }

        public async Task<Order?> GetOrderByPaymentId(int id)
        {
            var connection = _context.GetConnection();
            var transaction = _context.GetTransaction();

            var orderDictionary = new Dictionary<int, Order>();

            var sql = @"
            SELECT o.Id, o.CustomerName, o.PhoneNumber, o.Email, o.Date, o.Status, 
                   a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country, 
                   od.Id, od.ProductTitle, od.ProductAuthor, od.ProductDescription, od.ProductPrice, od.ProductQuantity, od.ProductCategoryName
            FROM OrdersForPayments o
            LEFT JOIN Addresses a ON o.CustomerAddressId = a.Id
            LEFT JOIN OrderDetailsForPayments od ON o.Id = od.OrderId
            WHERE o.PaymentId = @Id";

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
                }, new { Id = id }, transaction: transaction);
                return orderDictionary.Values.SingleOrDefault();
            }
            catch
            {
                _context.Rollback();
                throw;
            }
        }
    }
}
