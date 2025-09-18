namespace MyProjectTemplate.Core.Base;

public class BaseErrorResult : BaseResult
{
    public BaseErrorResult(bool success = false, string message = "", IEnumerable<BaseError>? errors = null)
    {
        Success = success;
        Message = message;
        Errors = errors is null ? new List<BaseError>() : new List<BaseError>(errors);
    }

    public List<BaseError> Errors { get; }
}

public class BaseError
{
    public string Field { get; set; }
    public string Error { get; set; }
}