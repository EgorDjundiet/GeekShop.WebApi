using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;
using System.Transactions;

namespace GeekShop.Services
{
    public class PaymentService : IPaymentService
    {
        IPaymentRepository _paymentRepository;
        IOrderService _orderService;
        AbstractValidator<SubmitPaymentIn> _paymentValidator;
        public PaymentService(IPaymentRepository paymentRepository, IOrderService orderService, AbstractValidator<SubmitPaymentIn> paymentValidator)
        {
            _paymentRepository = paymentRepository;
            _orderService = orderService;
            _paymentValidator = paymentValidator;
        }
        public async Task<Payment> Get(int id)
        {
            var payment = await _paymentRepository.Get(id);
            if(payment is null)
            {
                throw new GeekShopNotFoundException($"Invalid payment id: {id}");
            }
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            return await _paymentRepository.GetAll();
        }

        public async Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids)
        {
            ids = ids.Distinct();
            var payments = await _paymentRepository.GetByIds(ids);
            var invalidIds = ids.Where(id => !payments.Select(p => p.Id).Contains(id));
            if(invalidIds.Count() > 0)
            {
                string idsStr = string.Join(",", invalidIds);
                throw new GeekShopNotFoundException($"Invalid payment ids: {idsStr}");
            }
            return payments;
        }

        public async Task<Payment> Add(SubmitPaymentIn paymentIn)
        {
            var result = _paymentValidator.Validate(paymentIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }
            
            await _orderService.Get(paymentIn.OrderId!.Value);

            var expDate = paymentIn.CardDetails!.ExpDate!.Value;
            expDate = expDate.AddDays(DateTime.DaysInMonth(expDate.Year, expDate.Month) - expDate.Day);
            var accountNumber = new string('*',12) + paymentIn.CardDetails.AccountNumber!.Substring(12);
            var payment = new Payment()
            {   
                AccountNumber = accountNumber,
                Amount = paymentIn.Amount!.Value,
                Status = PaymentStatus.Submitted,
                OrderId = paymentIn.OrderId.Value,
                BillingAddress = new Address()
                {
                    Street = paymentIn.BillingAddress!.Street!,
                    City = paymentIn.BillingAddress!.City!,
                    State = paymentIn.BillingAddress!.State!,
                    ZipCode = paymentIn.BillingAddress!.ZipCode!,
                    Country = paymentIn.BillingAddress!.Country!
                }
            };

            var order = await _orderService.Get(payment.OrderId);

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var paymentFromRepository = await _paymentRepository.Add(payment);
                await _orderService.ChangeStatus(order.Id, OrderStatus.PendingPayment);               
                transactionScope.Complete();
                return paymentFromRepository;
            }           
        }

        public async Task Update(int id, SubmitPaymentIn paymentIn)
        {
            var result = _paymentValidator.Validate(paymentIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }

            var payment = await _paymentRepository.Get(id);
            if (payment is null)
            {
                throw new GeekShopNotFoundException($"Invalid payment id: {id}");
            }

            await _orderService.Get(paymentIn.OrderId!.Value);

            var expDate = paymentIn.CardDetails!.ExpDate!.Value;
            expDate = expDate.AddDays(DateTime.DaysInMonth(expDate.Year, expDate.Month) - expDate.Day);
            var accountNumber = new string('*', 12) + paymentIn.CardDetails.AccountNumber!.Substring(12);
            payment.AccountNumber = accountNumber;
            payment.Amount = paymentIn.Amount!.Value;
            payment.Status = PaymentStatus.Submitted;
            payment.OrderId = paymentIn.OrderId.Value;
            payment.BillingAddress = new Address()
            {
                Street = paymentIn.BillingAddress!.Street!,
                City = paymentIn.BillingAddress!.City!,
                State = paymentIn.BillingAddress!.State!,
                ZipCode = paymentIn.BillingAddress!.ZipCode!,
                Country = paymentIn.BillingAddress!.Country!
            };           
            await _paymentRepository.Update(payment);
        }

        public async Task Delete(int id)
        {
            if (await _paymentRepository.Get(id) is null)
            {
                throw new GeekShopNotFoundException($"Invalid payment id: {id}");
            }
            await _paymentRepository.Delete(id);
        }

        public async Task SeedData()
        {
            var expDate = DateTime.UtcNow;
            expDate = expDate.AddDays(DateTime.DaysInMonth(expDate.Year, expDate.Month) - expDate.Day);
            var payments = new List<SubmitPaymentIn>()
            {
                new SubmitPaymentIn()
                {
                    OrderId = 1,
                    Amount = 500,
                    BillingAddress = new SubmitAddressIn
                    {
                        Street = "Privet",
                        City = "London",
                        State = "London",
                        ZipCode = "24017",
                        Country = "UK"
                    },
                    CardDetails = new SubmitCardDetailsIn()
                    {
                        NameOnCard = "John Walker",
                        AccountNumber = "1111222233334444",
                        ExpDate = DateTime.UtcNow,
                        Cvv = "1234"
                    }
                },
                new SubmitPaymentIn()
                {
                    OrderId = 2,
                    Amount = 1000,
                    BillingAddress = new SubmitAddressIn
                    {
                        Street = "Komsomolskaya",
                        City = "Moscow",
                        State = "Moscow",
                        ZipCode = "46712",
                        Country = "Russia"
                    },
                    CardDetails = new SubmitCardDetailsIn()
                    {
                        NameOnCard = "Natalya Pushkin",
                        AccountNumber = "4444333322221111",
                        ExpDate = DateTime.UtcNow,
                        Cvv = "4309"
                    }
                },
                new SubmitPaymentIn()
                {
                    OrderId = 3,
                    Amount = 100,
                    BillingAddress = new SubmitAddressIn
                    {
                        Street = "Oderbergerstrasse",
                        City = "Berlin",
                        State = "Berlin",
                        ZipCode = "09321",
                        Country = "Germany"
                    },
                    CardDetails = new SubmitCardDetailsIn()
                    {
                        NameOnCard = "Alois Ramnstein",
                        AccountNumber = "9999888866667777",
                        ExpDate = DateTime.UtcNow,
                        Cvv = "963"
                    }
                }
            };
            using(var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var payment in payments)
                {
                    await Add(payment);
                }
                transactionScope.Complete();
            }           
        }
    }
}
