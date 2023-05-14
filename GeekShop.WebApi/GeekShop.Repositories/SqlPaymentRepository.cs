using Dapper;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;

namespace GeekShop.Repositories
{
    public class SqlPaymentRepository : IPaymentRepository
    {
        IDbContext _context;
        public SqlPaymentRepository(IDbContext context) 
        {
            _context = context;
        }
        public async Task<Payment?> Get(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    SELECT p.Id, p.Status, p.Amount, p.OrderId,
                           c.Id, c.NameOnCard, c.AccountNumber, c.ExpDate, c.Cvv,
                           a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country
                    FROM Payments as p
                    LEFT JOIN Cards as c ON p.CardId = c.Id
                    LEFT JOIN Addresses as a on p.BillingAddressId = a.Id
                    WHERE p.Id = @Id
                ";
                var paymentDict = new Dictionary<int, Payment>();
                await connection.QueryAsync<Payment,CardDetails,Address,Payment>(sql,(payment, cardDetails, address) =>
                {
                    payment.CardDetails = cardDetails;
                    payment.BillingAddress = address;
                    paymentDict.Add(payment.Id, payment);
                    return payment;
                }, new { Id = id});
                return paymentDict.Values.SingleOrDefault();
            }
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    SELECT p.Id, p.Status, p.Amount, p.OrderId,
                           c.Id, c.NameOnCard, c.AccountNumber, c.ExpDate, c.Cvv,
                           a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country
                    FROM Payments as p
                    LEFT JOIN Cards as c ON p.CardId = c.Id
                    LEFT JOIN Addresses as a on p.BillingAddressId = a.Id
                ";
                var paymentDict = new Dictionary<int,Payment>();
                await connection.QueryAsync<Payment, CardDetails, Address, Payment>(sql, (payment, cardDetails, address) =>
                {
                    payment.CardDetails = cardDetails;
                    payment.BillingAddress = address;
                    paymentDict.Add(payment.Id, payment);
                    return payment;
                });
                return paymentDict.Values;
            }
        }

        public async Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    SELECT p.Id, p.Status, p.Amount, p.OrderId,
                           c.Id, c.NameOnCard, c.AccountNumber, c.ExpDate, c.Cvv,
                           a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country
                    FROM Payments as p
                    LEFT JOIN Cards as c ON p.CardId = c.Id
                    LEFT JOIN Addresses as a on p.BillingAddressId = a.Id
                    WHERE p.Id in @Ids
                ";
                var paymentDict = new Dictionary<int,Payment>();
                await connection.QueryAsync<Payment, CardDetails, Address, Payment>(sql, (payment, cardDetails, address) =>
                {
                    payment.CardDetails = cardDetails;
                    payment.BillingAddress = address;
                    paymentDict.Add(payment.Id, payment);
                    return payment;
                }, new {Ids = ids.ToArray()});
                return paymentDict.Values;
            }
        }

        public async Task Add(Payment payment)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                
                var sqlPayment = @"
                    INSERT INTO Payments
                    (CardId, OrderId, BillingAddressId, Status, Amount)
                    VALUES(@CardId, @OrderId, @BillingAddressId, @Status, @Amount)";

                var sqlCard = @"
                    INSERT INTO Cards
                    (NameOnCard, AccountNumber, ExpDate, Cvv)
                    VALUES(@NameOnCard, @AccountNumber, @ExpDate, @Cvv)
                    SELECT SCOPE_IDENTITY()";

                var sqlAddress = @"
                    INSERT INTO Addresses
                    (Street, City, State, ZipCode, Country)
                    VALUES(@Street, @City, @State, @ZipCode, @Country)
                    SELECT SCOPE_IDENTITY()";

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //Card param
                        var parameters = new DynamicParameters();
                        parameters.Add("NameOnCard", payment.CardDetails.NameOnCard);
                        parameters.Add("AccountNumber", payment.CardDetails.AccountNumber);
                        parameters.Add("ExpDate", payment.CardDetails.ExpDate);
                        parameters.Add("Cvv", payment.CardDetails.Cvv);
                        var cardId = await connection.QueryFirstAsync<int>(sqlCard, parameters, transaction: transaction);

                        //Address param
                        parameters = new DynamicParameters();
                        parameters.Add("Street", payment.BillingAddress.Street);
                        parameters.Add("City", payment.BillingAddress.City);
                        parameters.Add("State", payment.BillingAddress.State);
                        parameters.Add("ZipCode", payment.BillingAddress.ZipCode);
                        parameters.Add("Country", payment.BillingAddress.Country);
                        var addressId = await connection.QueryFirstAsync<int>(sqlAddress, parameters, transaction: transaction);

                        //Payment param
                        parameters = new DynamicParameters();
                        parameters.Add("CardId", cardId);
                        parameters.Add("OrderId", payment.OrderId);
                        parameters.Add("BillingAddressId", addressId);
                        parameters.Add("Status", payment.Status.ToString());
                        parameters.Add("Amount", payment.Amount);
                        await connection.QueryAsync(sqlPayment,parameters,transaction:transaction);

                        transaction.Commit();
                    }
                    catch(Exception ex) 
                    {
                        transaction.Rollback();
                        throw new GeekShopException(ex.Message);
                    }
                }
                    
            }

        }

        public async Task Update(Payment payment)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();               

                var updatePayment = @"
                    UPDATE Payments
                    SET OrderId = @OrderId, Status = @Status, Amount = @Amount
                    WHERE Id = @Id";

                var updateCard = @"
                    UPDATE Cards
                    SET NameOnCard = @NameOnCard, AccountNumber = @AccountNumber, ExpDate = @ExpDate, Cvv = @Cvv
                    WHERE Id = @Id";

                var updateAddress = @"
                    UPDATE Addresses
                    SET Street = @Street, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country
                    WHERE Id = @Id";

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        //Card param
                        var parameters = new DynamicParameters();                       
                        parameters.Add("Id", payment.CardDetails.Id);
                        parameters.Add("NameOnCard", payment.CardDetails.NameOnCard);
                        parameters.Add("AccountNumber", payment.CardDetails.AccountNumber);
                        parameters.Add("ExpDate", payment.CardDetails.ExpDate);
                        parameters.Add("Cvv", payment.CardDetails.Cvv);
                        await connection.QueryAsync(updateCard, parameters, transaction: transaction);

                        //Address param
                        parameters = new DynamicParameters();
                        parameters.Add("Id", payment.BillingAddress.Id);
                        parameters.Add("Street", payment.BillingAddress.Street);
                        parameters.Add("City", payment.BillingAddress.City);
                        parameters.Add("State", payment.BillingAddress.State);
                        parameters.Add("ZipCode", payment.BillingAddress.ZipCode);
                        parameters.Add("Country", payment.BillingAddress.Country);                       
                        await connection.QueryAsync(updateAddress, parameters, transaction: transaction);

                        //Payment param
                        parameters = new DynamicParameters();
                        parameters.Add("Id", payment.Id);
                        parameters.Add("OrderId",payment.OrderId);
                        parameters.Add("Status", payment.Status.ToString());
                        parameters.Add("Amount", payment.Amount);
                        await connection.QueryAsync(updatePayment, parameters, transaction: transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new GeekShopException(ex.Message);
                    }
                }
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var payment = await Get(id);
                var sql = @"
                DELETE FROM Payments WHERE Id = @p_Id
                DELETE FROM Cards WHERE Id = @c_Id
                DELETE FROM Addresses WHERE Id = @a_Id";
                
                await connection.QueryAsync(sql, new { c_id = payment!.CardDetails.Id, a_id = payment!.BillingAddress.Id, p_id = id});
            }             
        }  
    }
}
