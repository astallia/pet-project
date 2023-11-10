using Newtonsoft.Json;
using System.Net;
using TFGames.Common.Exceptions;

namespace TFGames.Host.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BadRequestException badRequest)
            {
                await ExceptionHandler(badRequest, context, HttpStatusCode.BadRequest);
            }
            catch (ConflictException conflict)
            {
                await ExceptionHandler(conflict, context, HttpStatusCode.Conflict);
            }
            catch (InternalServerErrorException internalServerError)
            {
                await ExceptionHandler(internalServerError, context, HttpStatusCode.InternalServerError);
            }
            catch (NotFoundException notFound)
            {
                await ExceptionHandler(notFound, context, HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException unauthorized)
            {
                await ExceptionHandler(unauthorized, context, HttpStatusCode.Unauthorized);
            }
            catch (ForbiddenException forbidden)
            {
                await ExceptionHandler(forbidden, context, HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                await ExceptionHandler(ex, context, HttpStatusCode.InternalServerError);
            }
        }

        private async Task ExceptionHandler(Exception exception, HttpContext context, HttpStatusCode statusCode)
        {
            _logger.LogError(exception, "error during executing {context}", context.Request.Path.Value);

            var response = context.Response;
            response.ContentType = "application/json";

            string message = exception.Message;
            response.StatusCode = (int)statusCode;

            var result = JsonConvert.SerializeObject(message);

            await response.WriteAsync(result);
        }
    }
}
