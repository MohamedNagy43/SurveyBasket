using OneOf.Types;

namespace SurveyBasket.Api.Abstractions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert to problem of successful result");


        // produce problem details using Results
        IResult ProblemResult = Results.Problem(statusCode: result.Error.StatusCode,
            extensions: new Dictionary<string, object?>
            {
                {
                    "Errors",new[]{result.Error}
                }
            });

        ProblemDetails? problemDetails =
            ProblemResult.GetType().GetProperty(nameof(ProblemDetails))?.GetValue(ProblemResult) as ProblemDetails;

        return new ObjectResult(problemDetails);
    }


    public static ObjectResult ToProblem(this Error error)
    {
        if (error == Error.None)
            throw new InvalidOperationException("Cannot convert to problem of None Error");

        // produce problem details
        IResult ProblemResult = Results.Problem(statusCode: error.StatusCode,
            extensions: new Dictionary<string, object?>
            {
                {
                    "Errors",new[]{error}
                }
            });

        // using reflection to get the property value

        ProblemDetails? problemDetails =
            ProblemResult.GetType().GetProperty(nameof(ProblemDetails))?.GetValue(ProblemResult) as ProblemDetails;

        return new ObjectResult(problemDetails);
    }
}
