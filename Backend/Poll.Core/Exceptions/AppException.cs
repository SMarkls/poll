namespace Poll.Core.Exceptions;

public class AppException(string message, int statusCode = 400) : Exception(message)
{
    public readonly int StatusCode = statusCode;
}