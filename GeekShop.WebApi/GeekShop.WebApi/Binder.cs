using GeekShop.Repositories.Contracts;
using GeekShop.Repositories;
using GeekShop.Services.Contracts;
using GeekShop.Services;
using FluentValidation;
using GeekShop.Domain.Validators;
using GeekShop.Domain.ViewModels;
using GeekShop.Repositories.Contexts;

namespace GeekShop.WebApi
{
    public static class Binder
    {
        public static void RegisterDependencies(this IServiceCollection sc)
        {
            sc.AddScoped<IDbContext, DbContext>();
            sc.AddScoped<IProductService, ProductService>();
            sc.AddScoped<IProductRepository, SqlProductRepository>();
            sc.AddScoped<IOrderService, OrderService>();
            sc.AddScoped<IOrderRepository, SqlOrderRepository>();
            sc.AddScoped<ICustomerService, CustomerService>();
            sc.AddScoped<ICustomerRepository, SqlCustomerRepository>();
            sc.AddScoped<AbstractValidator<SubmitCustomerIn>, SubmitCustomerInValidator>();
            sc.AddScoped<AbstractValidator<SubmitOrderIn>, SumbitOrderInValidator>();
            sc.AddScoped<AbstractValidator<SubmitOrderDetailsIn>, SubmitOrderDetailsInValidator>();
            sc.AddScoped<AbstractValidator<SubmitProductIn>, SubmitProductInValidator>();           
        }
    }
}
