using GeekShop.Domain.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace GeekShop.WebApi.MiddlewareComponents
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (GeekShopNotFoundException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;                               
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Message = ex.Message}));
            }
            catch (GeekShopValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Message = ex.Message }));
            }
            catch(GeekShopDatabaseException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Message = ex.Message }));
            }
        }
    }
}
