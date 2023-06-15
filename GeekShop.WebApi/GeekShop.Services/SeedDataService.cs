using GeekShop.Domain.Exceptions;
using GeekShop.Domain;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contracts;
using GeekShop.Services.Contracts;

namespace GeekShop.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeedDataService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task SeedData()
        {
            await SeedCategories();
            await SeedCustomers();
            await SeedProducts();
            await SeedOrders();
            await SeedPayments();
            _unitOfWork.SaveChanges();
        }
        private async Task SeedCategories()
        {
            var submitCategories = new List<SubmitCategoryWithParentIn>()
            {
                new SubmitCategoryWithParentIn()
                {
                    Name = "Books"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Clothes"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Toys"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Jewellery"
                },
                new SubmitCategoryWithParentIn()
                {
                    Name = "Sporting Goods"
                },
            };

            foreach (var category in submitCategories)
            {
                await AddCategory(category);
            }
        }

        private async Task AddCategory(SubmitCategoryWithParentIn categoryIn)
        {
            var category = new CategoryWithParent
            {
                Name = categoryIn.Name!,
                Description = categoryIn.Description!,
                ParentId = categoryIn.ParentId
            };
            await _unitOfWork.Categories.Add(category);
        }

        private async Task SeedCustomers()
        {
            var customers = new List<SubmitCustomerIn>()
            {
                new SubmitCustomerIn()
                {
                    Name = "John Walker",
                    Address = new SubmitAddressIn
                    {
                        Street = "Privet",
                        City = "London",
                        State = "London",
                        ZipCode = "24017",
                        Country = "UK"
                    },
                    PhoneNumber = "+4477890345",
                    Email = "JohnWalker@gmail.com"
                },
                new SubmitCustomerIn()
                {
                    Name = "Natalya Pushkin",
                    Address = new SubmitAddressIn
                    {
                        Street = "Komsomolskaya",
                        City = "Moscow",
                        State = "Moscow",
                        ZipCode = "46712",
                        Country = "Russia"
                    },
                    PhoneNumber = "+777623908",
                    Email = "NatalyaPushkin@gmail.com"
                },
                new SubmitCustomerIn()
                {
                    Name = "Aloys Ramstein",
                    Address = new SubmitAddressIn
                    {
                        Street = "Oderbergerstrasse",
                        City = "Berlin",
                        State = "Berlin",
                        ZipCode = "09321",
                        Country = "Germany"
                    },
                    PhoneNumber = "+4977521653",
                    Email = "AloysRamstein@gmail.com"
                }
            };

            foreach (var customer in customers)
            {
                await AddCustomer(customer);
            }
        }

        private async Task AddCustomer(SubmitCustomerIn customerIn)
        {
            var customer = new Customer()
            {
                Name = customerIn.Name!,
                Address = new Address()
                {
                    Street = customerIn.Address!.Street!,
                    City = customerIn.Address!.City!,
                    State = customerIn.Address!.State!,
                    ZipCode = customerIn.Address!.ZipCode!,
                    Country = customerIn.Address!.Country!
                },
                PhoneNumber = customerIn.PhoneNumber,
                Email = customerIn.Email
            };
            await _unitOfWork.Customers.Add(customer);
        }

        private async Task SeedProducts()
        {
            var products = new List<SubmitProductIn>()
            {
                new SubmitProductIn
                {
                    Title = "Harry Potter And The Philosopher's Stone",
                    Author = "J.K.Rowling",
                    Description = "The story of the boy who lived",
                    Price = 13,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Description = "A powerful story of racial injustice and moral growth in a small Southern town",
                    Price = 20,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "1984",
                    Author = "George Orwell",
                    Description = "A dystopian novel depicting a totalitarian society where Big Brother watches every move",
                    Price = 7.99M,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "The Catcher in the Rye",
                    Author = "J.D.Salinger",
                    Description = "A coming-of-age novel following a disillusioned teenager's journey through New York City",
                    Price = 17,
                    CategoryId = 1
                },
                new SubmitProductIn
                {
                    Title = "To the Lighthouse",
                    Author = "Virginia Woolf",
                    Description = "A stream-of-consciousness novel exploring the intricacies of human emotions and relationships",
                    Price = 10,
                    CategoryId = 1
                }
            };

            foreach (var product in products)
            {
                await AddProduct(product);
            }
        }

        private async Task AddProduct(SubmitProductIn productIn)
        {
            var product = new Product()
            {
                Title = productIn.Title!,
                Author = productIn.Author!,
                Description = productIn.Description,
                Price = productIn.Price!.Value,
                CategoryName = (await _unitOfWork.Categories.Get(productIn.CategoryId!.Value))!.Name
            };
            await _unitOfWork.Products.Add(product);
        }

        private async Task SeedOrders()
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
                await AddOrder(order);
            }

        }

        private async Task AddOrder(SubmitOrderIn orderIn)
        {
            if (orderIn.CustomerId is not null)
            {
                var customer = await _unitOfWork.Customers.Get(orderIn.CustomerId.Value);

                orderIn.CustomerName = customer!.Name;
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

            var products = await _unitOfWork.Products.GetByIds(orderIn.Details!.Select(x => x.ProductId!.Value));

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
                    var product = products.Where(product => product.Id == detail.ProductId).Single();
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
            await _unitOfWork.Orders.Add(order);
        }

        private async Task SeedPayments()
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
                        ExpDate = expDate,
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
                        ExpDate = expDate,
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
                        ExpDate = expDate,
                        Cvv = "9683"
                    }
                }
            };

            foreach (var payment in payments)
            {
                await AddPayment(payment);
            }
        }

        private async Task AddPayment(SubmitPaymentIn paymentIn)
        {
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

            var order = (await _unitOfWork.Orders.Get(payment.OrderId.Value))!;
            await _unitOfWork.Orders.ChangeStatus(payment.OrderId.Value, OrderStatus.PendingPayment);
            order.Status = OrderStatus.PendingPayment;
            await _unitOfWork.Payments.Add(payment, order);
        }
    }
}
