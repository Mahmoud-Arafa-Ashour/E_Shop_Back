using Microsoft.AspNetCore.Mvc;

namespace E_Shop.Core.Abstractions;

public static class ResultExtension
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Can not converted to problem");

        var details = new ProblemDetails
        {
            Status = result.Error.StatueCode,
            Title = "An error occurred",
        };
        details.Extensions = new Dictionary<string, object?>
        {
            {
                "errors",
                new[] { result.Error.code, result.Error.description }
            }
        };

        return new ObjectResult(details)
        {
            StatusCode = result.Error.StatueCode
        };
    }
}
