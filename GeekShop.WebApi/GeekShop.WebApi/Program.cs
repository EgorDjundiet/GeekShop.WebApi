using GeekShop.Repositories;
using GeekShop.Repositories.Contracts;
using GeekShop.Services;
using GeekShop.Services.Contracts;
using GeekShop.WebApi.MiddlewareComponents;
using GeekShop.Repositories.Contexts;
using GeekShop.Domain.Validators;
using FluentValidation;
using GeekShop.Domain;
using GeekShop.Domain.ViewModels;

namespace GeekShop.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Add my own dependencies

            builder.Services.AddSingleton<Context>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository,SqlOrderRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ICustomerRepository, SqlCustomerRepository>();
            builder.Services.AddScoped<AbstractValidator<SubmitCustomerIn>,SubmitCustomerInValidator>();
            builder.Services.AddScoped<AbstractValidator<SubmitOrderIn>,SumbitOrderInValidator>();
            builder.Services.AddScoped<AbstractValidator<SubmitOrderDetailsIn>,SubmitOrderDetailsInValidator>();
            builder.Services.AddScoped<AbstractValidator<SubmitProductIn>,SubmitProductInValidator>();

            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlerMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}