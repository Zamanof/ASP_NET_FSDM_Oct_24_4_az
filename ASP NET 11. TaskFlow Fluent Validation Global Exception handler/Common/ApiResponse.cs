namespace ASP_NET_11._TaskFlow_Fluent_Validation_Global_Exception_handler.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public object? Errors { get; set; }

    public long? ExecutionTimeMs { get; set; }

    public static ApiResponse<T> SuccessResponse(
        T? data,
        string message = "Operation executed successfully",
        long? executionTimeMs = null)
        => new()
        {
            Success = true,
            Message = message,
            Data = data,
            ExecutionTimeMs = executionTimeMs
        };

    public static ApiResponse<T> ErrorResponse(
        string message,
        object? errors = null,
        long? executionTimeMs = null)
        => new()
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors,
            ExecutionTimeMs = executionTimeMs
        };
}
