using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.Validators;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;

namespace GeekShop.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly AbstractValidator<SubmitOrderIn> _orderValidator;

        public OrderService(IOrderRepository repository, IProductRepository productRepository, ICustomerRepository customerRepository, AbstractValidator<SubmitOrderIn> orderValidator)
        {
            _orderRepository = repository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _orderValidator = orderValidator;
        }


        public async Task Add(SubmitOrderIn orderIn)
        {
            if (orderIn.CustomerId is not null)
            {
                var customer = await _customerRepository.Get(orderIn.CustomerId.Value);

                if (customer is null)
                {
                    throw new GeekShopNotFoundException($"Invalid customer id: {orderIn.CustomerId}.");
                }

                orderIn.CustomerName = customer.Name;
                orderIn.CustomerAddress = customer.Address;
                orderIn.PhoneNumber = customer.PhoneNumber;
                orderIn.Email = customer.Email;
            }

            var result = _orderValidator.Validate(orderIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }

            var validProducts = await _productRepository.GetByIds(orderIn.Details!.Select(x => x.ProductId!.Value));
            var validProductsIds = validProducts.Select(x => x.Id);

            var invalidProductIds = orderIn.Details!.Select(x => x.ProductId!.Value).Except(validProductsIds);
            if (invalidProductIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidProductIds);
                throw new GeekShopNotFoundException($"Invalid product ids: {unfoundIds}");
            }

            
            var order = new Order()
            {
                CustomerName = orderIn.CustomerName!,
                CustomerAddress = orderIn.CustomerAddress!,
                PhoneNumber = orderIn.PhoneNumber,
                Email = orderIn.Email,
                Details = orderIn.Details!.Select(detail => new OrderDetails()
                {
                    Product = validProducts.Where(product => product.Id == detail.ProductId).Single(),
                    ProductQuantity = detail.ProductQuantity!.Value
                }).ToList()
            };

            await _orderRepository.Add(order);
        }

        public async Task Delete(int id)
        {
            var order = await _orderRepository.Get(id);
            if (order is null)
            {
                throw new GeekShopNotFoundException($"Invalid order id {id}.");
            }
            await _orderRepository.Delete(id);
        }

        public async Task<Order> Get(int id)
        {
            var order = await _orderRepository.Get(id);
            if (order is null)
            {
                throw new GeekShopNotFoundException($"Invalid order id {id}.");
            }
            return order;
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderRepository.GetAll();
        }

        public async Task Update(int id, SubmitOrderIn orderIn)
        {
            if (orderIn.CustomerId is not null)
            {
                var customer = await _customerRepository.Get(orderIn.CustomerId.Value);

                if (customer is null)
                {
                    throw new GeekShopNotFoundException($"Invalid customer id: {orderIn.CustomerId}.");
                }

                orderIn.CustomerName = customer.Name;
                orderIn.CustomerAddress = customer.Address;
                orderIn.PhoneNumber = customer.PhoneNumber;
                orderIn.Email = customer.Email;
            }

            var result = _orderValidator.Validate(orderIn);
            if (!result.IsValid)
            {
                throw new GeekShopValidationException(result.ToString());
            }
            if(await _orderRepository.Get(id) is null)
            {
                throw new GeekShopNotFoundException($"Invalid order id: {id}.");
            }

            var validProducts = await _productRepository.GetByIds(orderIn.Details!.Select(x => x.ProductId!.Value));
            var validProductsIds = validProducts.Select(x => x.Id);

            var invalidProductIds = orderIn.Details!.Select(x => x.ProductId!.Value).Except(validProductsIds);
            if (invalidProductIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidProductIds);
                throw new GeekShopNotFoundException($"Invalid product ids: {unfoundIds}.");
            }

            var order = new Order()
            {
                Id = id,
                CustomerName = orderIn.CustomerName!,
                CustomerAddress = orderIn.CustomerAddress!,
                PhoneNumber = orderIn.PhoneNumber,
                Email = orderIn.Email,
                Details = orderIn.Details!.Select(detail => new OrderDetails()
                {
                    Product = validProducts.Where(product => product.Id == detail.ProductId).Single(),
                    ProductQuantity = detail.ProductQuantity!.Value
                }).ToList()
            };
            
            await _orderRepository.Update(order);
        }

        public async Task<IEnumerable<Order>> GetByIds(IEnumerable<int> ids)
        {
            ids = ids.Distinct();
            var orders = await _orderRepository.GetByIds(ids);
            var invalidIds = ids.Where(id => !orders.Select(p => p.Id).Contains(id));
            if (invalidIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidIds);
                throw new GeekShopNotFoundException($"Invalid order ids: {unfoundIds}.");
            }
            return orders;
        }
    }
}
