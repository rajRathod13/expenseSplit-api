namespace ExpenseSplit.Common.ResponseDTOs;

public class BaseResponse<T> 
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public IDictionary<string, string[]> Errors { get; set; }
    public T Data { get; set; }

    public static BaseResponse<T> Success(T data, string message = "Success")
        => new() { IsSuccess = true, Data = data, Message = message };

    public static BaseResponse<T> Fail( string message, IDictionary<string, string[]> errors = null)
       => new() { IsSuccess = false, Message = message, Errors = errors };

    
}
