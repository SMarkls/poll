namespace Poll.Api.Models.Common;

public class ApiErrorModel
{
    public int StatusCode { get; init; }
    public string Message { get; init; }
    public string TraceIdentifier { get; init; }
}