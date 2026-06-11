namespace OweMeApi.Common;

public class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; set; } = statusCode;
    public Dictionary<string, object?> Extensions { get; set; } = [];
}
