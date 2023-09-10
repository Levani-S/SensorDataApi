using SensorDataApi.Exceptions;


namespace SensorDataApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger; 

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger; 
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AlreadyExistsException aex)
            {
                await HandleExceptionAsync(context, aex, StatusCodes.Status400BadRequest);
            }

            catch (NotFoundException nex)
            {
                await HandleExceptionAsync(context, nex, StatusCodes.Status400BadRequest);
            }
            catch (CustomException cex)
            {
                await HandleExceptionAsync(context, cex, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(new NewRecord(
                context.Response.StatusCode,
                exception.Message
            ).ToString());
        }
    }
    internal record NewRecord(int StatusCode, string Message);
}
