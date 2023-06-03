﻿using Dapper;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Transactions;

namespace GeekShop.Repositories
{
    public class SqlCustomerRepository : ICustomerRepository
    {
        private readonly IDbContext _context;
        public SqlCustomerRepository(IDbContext context)
        {
            _context = context;
        }
        public async Task Add(Customer customer)
        {           
            using(IDbConnection connection = _context.CreateConnection())
            {
                var sql = @"
                    BEGIN TRAN
                        INSERT INTO Addresses
                        (Street, City, State, ZipCode, Country)
                        VALUES(@Street, @City, @State, @ZipCode, @Country)

                        DECLARE @AddressIdVariable INT
                        SELECT @AddressIdVariable = SCOPE_IDENTITY() 
                        
                        INSERT INTO Customers
                        (Name, AddressId, PhoneNumber, Email)
                        VALUES(@Name,@AddressIdVariable,@PhoneNumber,@Email)
                    COMMIT TRAN
                ";

                var dictParam = new DynamicParameters();
                dictParam.Add("Street", customer.Address.Street);
                dictParam.Add("City", customer.Address.City);
                dictParam.Add("State", customer.Address.State);
                dictParam.Add("ZipCode", customer.Address.ZipCode);
                dictParam.Add("Country", customer.Address.Country);
                dictParam.Add("Name", customer.Name);
                dictParam.Add("PhoneNumber", customer.PhoneNumber);
                dictParam.Add("Email", customer.Email);

                try
                {                     
                    await connection.QueryAsync(sql, dictParam);
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }                                                     
            }
        }

        public async Task Delete(int id)
        {
            var customer = await Get(id);
            var sql = @"
                BEGIN TRAN
                    DELETE FROM Customers WHERE Id = @c_Id
                    DELETE FROM Addresses WHERE Id = @a_Id
                COMMIT TRAN";

            using (IDbConnection connection = _context.CreateConnection())
            {
                try
                {
                    await connection.QueryAsync(sql, new { c_Id = id, a_id = customer!.Address.Id });
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problem with database");
                }
                
            }
        }

        public async Task<Customer?> Get(int id)
        {
            var sql = @"
                SELECT c.Id, c.Name, c.PhoneNumber, c.Email, a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country FROM Customers c
                JOIN Addresses a ON c.AddressId = a.Id
                WHERE c.Id = @Id";

            using (IDbConnection connection = _context.CreateConnection())
            {
                try
                {
                    return (await connection.QueryAsync<Customer, Address, Customer>(sql, (customer, address) =>
                    {
                        customer.Address = address;
                        return customer;
                    }, new { Id = id })).SingleOrDefault();
                }
                catch 
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
                
            }
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            var sql = @"
                SELECT c.Id, c.Name, c.PhoneNumber, c.Email, a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country FROM Customers c
                JOIN Addresses a ON c.AddressId = a.Id";

            using (IDbConnection connection = _context.CreateConnection())
            {
                try
                {
                    return await connection.QueryAsync<Customer, Address, Customer>(sql, (customer, address) =>
                    {
                        customer.Address = address;
                        return customer;
                    });
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
                
            }
        }

        public async Task<IEnumerable<Customer>> GetByIds(IEnumerable<int> ids)
        {
            var sql = @"
                SELECT c.Id, c.Name, c.PhoneNumber, c.Email, a.Id, a.Street, a.City, a.State, a.ZipCode, a.Country FROM Customers c
                JOIN Addresses a ON c.AddressId = a.Id
                WHERE c.Id in @Ids";

            using (IDbConnection connection = _context.CreateConnection())
            {
                try
                {
                    return await connection.QueryAsync<Customer, Address, Customer>(sql, (customer, address) =>
                    {
                        customer.Address = address;
                        return customer;
                    }, new { Ids = ids.ToArray() });
                }
                catch 
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
            }
        }

        public async Task Update(Customer customer)
        {           
            using (IDbConnection connection = _context.CreateConnection())
            {
                var sql = @"
                    BEGIN TRAN
                        UPDATE Customers
                        SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email
                        WHERE Id = @Id

                        DECLARE @AddressIdVariable INT
                        SELECT @AddressIdVariable = AddressId FROM Customers WHERE Id = @Id

                        UPDATE Addresses
                        SET Street = @Street, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country
                        WHERE Id = @AddressIdVariable
                    COMMIT TRAN
                ";
                
                try
                {
                    var dictParam = new DynamicParameters();
                    dictParam.Add("Id", customer.Id);
                    dictParam.Add("Name", customer.Name);
                    dictParam.Add("PhoneNumber", customer.PhoneNumber);
                    dictParam.Add("Email", customer.Email);
                    dictParam.Add("Street", customer.Address.Street);
                    dictParam.Add("City", customer.Address.City);
                    dictParam.Add("State", customer.Address.State);
                    dictParam.Add("ZipCode", customer.Address.ZipCode);
                    dictParam.Add("Country", customer.Address.Country);

                    await connection.QueryAsync(sql, dictParam);                
                }
                catch
                {
                    throw new GeekShopDatabaseException("Problems with database");
                }
                
            }
        }
    }
}
