using GeekShop.WebApi.MiddlewareComponents;
using GeekShop.Domain.Settings;

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
            builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.BasePosition));
            builder.Services.AddSingleton<IDbSettings, DbSettings>();
            builder.Services.RegisterDependencies();
            // Settings
            

            var app = builder.Build();
            var scope = app.Services.CreateScope();
            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionHandlerMiddleware>();
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