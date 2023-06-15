using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contexts;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;

namespace GeekShop.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AbstractValidator<SubmitPaymentIn> _paymentValidator;
        public PaymentService(IUnitOfWork unitOfWork, AbstractValidator<SubmitPaymentIn> paymentValidator)
        {
            _unitOfWork = unitOfWork;
            _paymentValidator = paymentValidator;
        }
        public async Task<Payment> Get(int id)
        {
            var payment = await _unitOfWork.Payments.Get(id);
            if(payment is null)
            {
                throw new NotFoundException($"Invalid payment id: {id}");
            }
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            return await _unitOfWork.Payments.GetAll();
        }

        public async Task<IEnumerable<Payment>> GetByIds(IEnumerable<int> ids)
        {
            ids = ids.Distinct();
            var payments = await _unitOfWork.Payments.GetByIds(ids);
            var invalidIds = ids.Where(id => !payments.Select(p => p.Id).Contains(id));
            if(invalidIds.Count() > 0)
            {
                string idsStr = string.Join(",", invalidIds);
                throw new NotFoundException($"Invalid payment ids: {idsStr}");
            }
            return payments;
        }

        public async Task<Payment> Add(SubmitPaymentIn paymentIn)
        {
            var result = _paymentValidator.Validate(paymentIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }

            var order = await _unitOfWork.Orders.Get(paymentIn.OrderId!.Value);
            if (order is null)
            {
                throw new NotFoundException($"Invalid order id: {paymentIn.OrderId}");
            }
            if (order.Status == OrderStatus.PendingPayment)
            {
                throw new AlreadyUsedException($"The order is already used by different payment");
            }

            var expDate = paymentIn.CardDetails!.ExpDate!.Value;
            expDate = expDate.AddDays(DateTime.DaysInMonth(expDate.Year, expDate.Month) - expDate.Day);
            var accountNumber = new string('*', 12) + paymentIn.CardDetails.AccountNumber!.Substring(12);
            var payment = new Payment()
            {
                AccountNumber = accountNumber,
                Amount = paymentIn.Amount!.Value,
                Status = PaymentStatus.Submitted,
                OrderId = paymentIn.OrderId!.Value,
                BillingAddress = new Address()
                {
                    Street = paymentIn.BillingAddress!.Street!,
                    City = paymentIn.BillingAddress!.City!,
                    State = paymentIn.BillingAddress!.State!,
                    ZipCode = paymentIn.BillingAddress!.ZipCode!,
                    Country = paymentIn.BillingAddress!.Country!
                }
            };

            await _unitOfWork.Orders.ChangeStatus(order.Id, OrderStatus.PendingPayment);
            order.Status = OrderStatus.PendingPayment;
            var id = await _unitOfWork.Payments.Add(payment, order);

            payment = await Get(id);
            _unitOfWork.SaveChanges();
            return payment;
        }

        public async Task Update(int id, SubmitPaymentIn paymentIn)
        {
            var result = _paymentValidator.Validate(paymentIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            var payment = await _unitOfWork.Payments.Get(id);
            if (payment is null)
            {
                throw new NotFoundException($"Invalid payment id: {id}");
            }
            await _unitOfWork.Orders.ChangeStatus(id, OrderStatus.Placed);
            var order = await _unitOfWork.Orders.Get(paymentIn.OrderId!.Value);
            if (order is null)
            {
                throw new NotFoundException($"Invalid order id: {payment.OrderId}");
            }
            if(order.Status == OrderStatus.PendingPayment)
            {
                throw new AlreadyUsedException($"The order is already used by different payment");
            }
            var expDate = paymentIn.CardDetails!.ExpDate!.Value;
            expDate = expDate.AddDays(DateTime.DaysInMonth(expDate.Year, expDate.Month) - expDate.Day);
            var accountNumber = new string('*', 12) + paymentIn.CardDetails.AccountNumber!.Substring(12);
            payment.AccountNumber = accountNumber;
            payment.Amount = paymentIn.Amount!.Value;
            payment.Status = PaymentStatus.Submitted;
            payment.OrderId = paymentIn.OrderId!.Value;
            payment.BillingAddress = new Address()
            {
                Street = paymentIn.BillingAddress!.Street!,
                City = paymentIn.BillingAddress!.City!,
                State = paymentIn.BillingAddress!.State!,
                ZipCode = paymentIn.BillingAddress!.ZipCode!,
                Country = paymentIn.BillingAddress!.Country!
            };
            
            await _unitOfWork.Payments.Update(payment, order);
            _unitOfWork.SaveChanges();
        }

        public async Task Delete(int id)
        {
            if (await _unitOfWork.Payments.Get(id) is null)
            {
                throw new NotFoundException($"Invalid payment id: {id}");
            }
            await _unitOfWork.Orders.ChangeStatus(id, OrderStatus.Placed);
            await _unitOfWork.Payments.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public async Task<Order> GetOrderByPaymentId(int id)
        {
            var order = await _unitOfWork.Payments.GetOrderByPaymentId(id);
            if(order == null)
            {
                throw new NotFoundException($"Invalid order id: {id}");
            }
            return order!;
        }
    }
}
