using Dapper;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using System.Transactions;

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
                    }, new { Id = id })).SingleOrDefault();
                }
                catch 
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }                
            }
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            using (var connection = _context.CreateConnection())
            {
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
                    }));
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
               
            }
        }

        public async Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids)
        {
            using (var connection = _context.CreateConnection())
            {
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
                    }, new { Ids = ids.ToArray() }));
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }                
            }
        }

        public async Task<Payment> Add(Payment payment)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    BEGIN TRAN
                        INSERT INTO Addresses
                        (Street, City, State, ZipCode, Country)
                        VALUES(@Street, @City, @State, @ZipCode, @Country)
                        
                        DECLARE @AddressIdVariable INT 
                        SELECT @AddressIdVariable = SCOPE_IDENTITY()

                        INSERT INTO Payments
                        (AccountNumber, OrderId, BillingAddressId, Status, Amount)
                        VALUES(@AccountNumber, @OrderId, @AddressIdVariable, @Status, @Amount)
                        SELECT SCOPE_IDENTITY();
                    COMMIT TRAN
                ";

                var parameters = new DynamicParameters();
                parameters.Add("Street", payment.BillingAddress.Street);
                parameters.Add("City", payment.BillingAddress.City);
                parameters.Add("State", payment.BillingAddress.State);
                parameters.Add("ZipCode", payment.BillingAddress.ZipCode);
                parameters.Add("Country", payment.BillingAddress.Country);
                parameters.Add("AccountNumber", payment.AccountNumber);
                parameters.Add("OrderId", payment.OrderId);
                parameters.Add("Status", payment.Status.ToString());
                parameters.Add("Amount", payment.Amount);

                try
                {
                    payment.Id = await connection.QuerySingleAsync<int>(sql, parameters);
                    return (await Get(payment.Id))!;
                }
                catch
                {                   
                    throw new GeekShopDatabaseException("Problems with database");
                }
                                   
            }
        }

        public async Task Update(Payment payment)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    BEGIN TRAN
                        UPDATE Payments
                        SET OrderId = @OrderId, Status = @Status, Amount = @Amount, AccountNumber = @AccountNumber
                        WHERE Id = @Id

                        DECLARE @AddressIdVariable INT 
                        SELECT @AddressIdVariable = BillingAddressId FROM Payments WHERE Id = @Id

                        UPDATE Addresses
                        SET Street = @Street, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country
                        WHERE Id = @AddressIdVariable                        
                    COMMIT TRAN
                ";

                var parameters = new DynamicParameters();
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

                try
                {                                       
                    await connection.QueryAsync(sql, parameters);                
                }
                catch 
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var payment = await Get(id);
                var sql = @"
                BEGIN TRAN
                    DELETE FROM Payments WHERE Id = @p_Id
                    DELETE FROM Addresses WHERE Id = @a_Id
                COMMIT TRAN";

                try
                {
                    await connection.QueryAsync(sql, new {a_Id = payment!.BillingAddress.Id, p_Id = id});
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
            }             
        }  
    }
}
