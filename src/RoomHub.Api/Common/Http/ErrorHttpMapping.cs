using Microsoft.AspNetCore.Mvc;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.Http;

/// <summary>The single place where a domain Error becomes an HTTP ProblemDetails response.</summary>
public static class ErrorHttpMapping
{
    public static int ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    public static IResult ToProblem(this Error error)
    {
        var problem = new ProblemDetails
        {
            Status = error.Type.ToStatusCode(),
            Title = error.Code,
            Detail = error.Message
        };
        problem.Extensions["code"] = error.Code;

        // Written as application/problem+json.
        return Results.Problem(problem);
    }
}
