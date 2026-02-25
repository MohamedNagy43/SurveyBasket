namespace SurveyBasket.Api.Errors;

public static class Errors<T> where T : class
{
    public static Error NotFound => 
        new Error($"{typeof(T).Name}.NotFound", $"No {typeof(T).Name} Found With this Key");

}
