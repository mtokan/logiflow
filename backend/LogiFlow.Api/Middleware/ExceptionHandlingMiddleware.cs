using LogiFlow.Application.Deliveries;
using LogiFlow.Application.Workflow;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DeliveryNotFoundException exception)
        {
            await WriteProblemDetailsAsync(context,
                StatusCodes.Status404NotFound,
                "Delivery not found",
                exception.Message);
        }
        catch (InvalidWorkflowTransitionException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Invalid workflow transition",
                exception.Message,
                new Dictionary<string, object?>
                {
                    ["currentState"] = exception.CurrentState.ToString(),
                    ["targetState"] = exception.TargetState.ToString()
                });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred.");

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        Dictionary<string, object?>? extensions = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (extensions is not null)
            foreach (var extension in extensions)
                problemDetails.Extensions[extension.Key] = extension.Value;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}