namespace Domain.Abstractions.Result;

public class Result
{
    public bool IsSuccess { get; private set; }
    public Error Error { get; private set; }

    public bool IsError => !IsSuccess;

    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Failure(Error error) => new(false, error);

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, Error.None);

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullError);

    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue>(TValue? value, Error error) : Result(value is not null, error)
{
    public TValue Value
    {
        get
        {
            if (value == null)
            {
                throw new InvalidOperationException("Trying to get a value from a null object.");
            }

            return value;
        }
    }

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}