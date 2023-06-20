using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.Exceptions;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contexts;
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


        public async Task<Order> Add(SubmitOrderIn orderIn)
        {
            if (orderIn.CustomerId is not null)
            {
                var customer = await _customerRepository.Get(orderIn.CustomerId.Value);

                if (customer is null)
                {
                    throw new NotFoundException($"Invalid customer id: {orderIn.CustomerId}.");
                }

                orderIn.CustomerName = customer.Name;
                orderIn.CustomerAddress = new SubmitAddressIn 
                {
                    Street = customer.Address.Street,
                    City = customer.Address.City,
                    State = customer.Address.State,
                    ZipCode = customer.Address.ZipCode,
                    Country = customer.Address.Country
                };
                orderIn.PhoneNumber = customer.PhoneNumber;
                orderIn.Email = customer.Email;
            }

            var result = _orderValidator.Validate(orderIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }

            var validProducts = await _productRepository.GetByIds(orderIn.Details!.Select(x => x.ProductId!.Value));
            var validProductsIds = validProducts.Select(x => x.Id);

            var invalidProductIds = orderIn.Details!.Select(x => x.ProductId!.Value).Except(validProductsIds);
            if (invalidProductIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidProductIds);
                throw new NotFoundException($"Invalid product ids: {unfoundIds}");
            }


            var order = new Order()
            {
                CustomerName = orderIn.CustomerName!,
                CustomerAddress = new Address
                {
                    Street = orderIn.CustomerAddress!.Street!,
                    City = orderIn.CustomerAddress!.City!,
                    State = orderIn.CustomerAddress!.State!,
                    ZipCode = orderIn.CustomerAddress!.ZipCode!,
                    Country = orderIn.CustomerAddress!.Country!
                },
                PhoneNumber = orderIn.PhoneNumber,
                Email = orderIn.Email,
                Date = DateTime.UtcNow,
                Details = orderIn.Details!.Select(detail =>
                {
                    var product = validProducts.Where(product => product.Id == detail.ProductId).Single();
                    var orderDetail = new OrderDetails()
                    {
                        ProductTitle = product.Title,
                        ProductAuthor = product.Author,
                        ProductDescription = product.Description,
                        ProductCategoryName = product.CategoryName,
                        ProductPrice = product.Price,
                        ProductQuantity = detail.ProductQuantity!.Value
                    };
                    return orderDetail;
                }).ToList(),
                Status = OrderStatus.Placed
            };
            var id = await _orderRepository.Add(order);
            return await Get(id);
        }

        public async Task Delete(int id)
        {
            var order = await _orderRepository.Get(id);
            if (order is null)
            {
                throw new NotFoundException($"Invalid order id {id}.");
            }
            await _orderRepository.Delete(id);
        }

        public async Task<Order> Get(int id)
        {
            var order = await _orderRepository.Get(id);
            if (order is null)
            {
                throw new NotFoundException($"Invalid order id {id}.");
            }
            return order;
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderRepository.GetAll();
        }
        public async Task ChangeStatus(int id, OrderStatus status)
        {
            await _orderRepository.ChangeStatus(id,status);
        }
        public async Task Update(int id, SubmitOrderIn orderIn)
        {
            if (orderIn.CustomerId is not null)
            {
                var customer = await _customerRepository.Get(orderIn.CustomerId.Value);

                if (customer is null)
                {
                    throw new NotFoundException($"Invalid customer id: {orderIn.CustomerId}.");
                }

                orderIn.CustomerName = customer.Name;
                orderIn.CustomerAddress = new SubmitAddressIn
                {
                    Street = customer.Address.Street,
                    City = customer.Address.City,
                    State = customer.Address.State,
                    ZipCode = customer.Address.ZipCode,
                    Country = customer.Address.Country
                };
                orderIn.PhoneNumber = customer.PhoneNumber;
                orderIn.Email = customer.Email;
            }

            var result = _orderValidator.Validate(orderIn);
            if (!result.IsValid)
            {
                throw new Domain.Exceptions.ValidationException(result.ToString());
            }
            if(await _orderRepository.Get(id) is null)
            {
                throw new NotFoundException($"Invalid order id: {id}.");
            }

            var validProducts = await _productRepository.GetByIds(orderIn.Details!.Select(x => x.ProductId!.Value));
            var validProductsIds = validProducts.Select(x => x.Id);

            var invalidProductIds = orderIn.Details!.Select(x => x.ProductId!.Value).Except(validProductsIds);
            if (invalidProductIds.Count() > 0)
            {
                var unfoundIds = string.Join(",", invalidProductIds);
                throw new NotFoundException($"Invalid product ids: {unfoundIds}.");
            }

            var order = new Order()
            {
                Id = id,
                CustomerName = orderIn.CustomerName!,
                CustomerAddress = new Address
                {
                    Street = orderIn.CustomerAddress!.Street!,
                    City = orderIn.CustomerAddress!.City!,
                    State = orderIn.CustomerAddress!.State!,
                    ZipCode = orderIn.CustomerAddress!.ZipCode!,
                    Country = orderIn.CustomerAddress!.Country!
                },
                PhoneNumber = orderIn.PhoneNumber,
                Email = orderIn.Email,
                Details = orderIn.Details!.Select(detail =>
                {
                    var product = validProducts.Where(product => product.Id == detail.ProductId).Single();
                    var orderDetail = new OrderDetails() 
                    { 
                        ProductTitle = product.Title,
                        ProductAuthor = product.Author,
                        ProductDescription = product.Description,
                        ProductCategoryName = product.CategoryName,
                        ProductPrice = product.Price,
                        ProductQuantity = detail.ProductQuantity!.Value
                    };
                    return orderDetail;
                }
                ).ToList()              
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
                throw new NotFoundException($"Invalid order ids: {unfoundIds}.");
            }
            return orders;
        }

        public async Task SeedData()
        {
            var orders = new List<SubmitOrderIn>()
            {
                new SubmitOrderIn()
                {
                    CustomerId = 1,
                    Details = new List<SubmitOrderDetailsIn>()
                    {
                        new SubmitOrderDetailsIn()
                        {
                            ProductId = 1,
                            ProductQuantity = 1
                        },
                        new SubmitOrderDetailsIn()
                        {
                            ProductId = 2,
                            ProductQuantity = 1
                        }
                    }
                },
                new SubmitOrderIn()
                {
                    CustomerId = 2,
                    Details = new List<SubmitOrderDetailsIn>()
                    {
                        new SubmitOrderDetailsIn()
                        {
                            ProductId = 3,
                            ProductQuantity = 2
                        },
                        new SubmitOrderDetailsIn()
                        {
                            ProductId = 1,
                            ProductQuantity = 5
                        }
                    }
                },
                new SubmitOrderIn()
                {
                    CustomerId = 3,
                    Details = new List<SubmitOrderDetailsIn>()
                    {
                        new SubmitOrderDetailsIn()
                        {
                            ProductId = 5,
                            ProductQuantity = 3
                        },
                        new SubmitOrderDetailsIn()
                        {
                            ProductId = 2,
                            ProductQuantity = 1
                        }
                    }
                }
            };

            foreach (var order in orders)
            {
                await Add(order);
            }
        }
    }
}
