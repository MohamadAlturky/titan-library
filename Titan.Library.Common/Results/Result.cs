namespace Titan.Library.Common.Results;

public class Result
{
    public bool IsSuccess { get; set; }
    public string MessageCode { get; set; } = string.Empty;

    protected Result() { }

    public static Result Success(string messageCode)
    {
        return new Result { IsSuccess = true, MessageCode = messageCode };
    }

    public static Result Fail(string messageCode)
    {
        return new Result { IsSuccess = false, MessageCode = messageCode };
    }
}

public class Result<T> : Result
{
    public T Data { get; set; }

    public static Result<T> Success(T data, string messageCode)
    {
        return new Result<T> { IsSuccess = true, MessageCode = messageCode, Data = data };
    }

    public new static Result<T> Fail(string messageCode)
    {
        return new Result<T> { IsSuccess = false, MessageCode = messageCode };
    }
    public static implicit operator Result<T>(string messageCode) => Fail(messageCode);
}
