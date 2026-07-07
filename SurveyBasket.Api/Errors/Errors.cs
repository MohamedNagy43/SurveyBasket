namespace SurveyBasket.Api.Errors;

public static class Errors<T> where T : class
{
    public static Error NotFound =>
        new Error($"{typeof(T).Name}.NotFound", $"No {typeof(T).Name} Found With this Key", StatusCodes.Status404NotFound);
    public static Error NotAllowed =>
        new Error($"NotAllowed", $"Action you are taking not allowed", StatusCodes.Status400BadRequest);

}
