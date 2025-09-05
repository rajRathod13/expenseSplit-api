using ExpenseSplit.Common.ResponseDTOs;
using System.Text.Json;

namespace ExpenseSplit.API;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context) {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse(500, "An unexpected error occurred.");

            if (_env.IsDevelopment())
            {
                response = new ErrorResponse(500,ex.Message, new List<string> { ex.StackTrace ?? ""});
            };

            switch (ex)
            {
                default:
                    context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                    break;
            }

            var json = JsonSerializer.Serialize<ErrorResponse>(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.Response.WriteAsync(json);
        }
    }
}
