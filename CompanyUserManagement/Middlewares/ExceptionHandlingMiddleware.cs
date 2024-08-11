using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CompanyUserManagement.Exceptions;
using static CompanyUserManagement.Exceptions.Exceptions;

namespace CompanyUserManagement.Middlewares
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
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Bad request error");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (ConflictException ex)
            {
                _logger.LogError(ex, "Conflict error");
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogError(ex, "Unauthorized error");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (InternalServerErrorException ex)
            {
                _logger.LogError(ex, "Internal server error");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (NotFoundException notFoundEx)
            {
                _logger.LogError(notFoundEx, "Not found error");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { error = notFoundEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}

