using SensorDataApi.Exceptions;
using System.Net;
using System.Text.Json;

namespace SensorDataApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _request;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private const string ErrorMessageTemplate = "An error occurred: {ErrorMessage}";


        public ErrorHandlerMiddleware(RequestDelegate request, ILogger<ErrorHandlerMiddleware> logger)
        {
            _request = request;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _request(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                ProcessingResponse(error, response);

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }

        private void ProcessingResponse(Exception error, HttpResponse response)
        {
            if (error is CustomException)
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            else
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError(ErrorMessageTemplate, error.Message);
        }
    }
}
