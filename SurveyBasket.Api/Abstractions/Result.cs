using Microsoft.OpenApi.Models;

namespace SurveyBasket.Api.Abstractions;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException("Arguments not Suitable");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; } = default!;

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}
public class Result<TValue> : Result
{
    private readonly TValue? _value;
    public Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        if ((isSuccess && value is null) || (!isSuccess && value is not null))
            throw new InvalidOperationException("Arguments not Suitable");

        _value = value;
    }

    public TValue Value => IsSuccess ?
        _value! : throw new InvalidOperationException("Can not Read A Value of Failure Operation");
}