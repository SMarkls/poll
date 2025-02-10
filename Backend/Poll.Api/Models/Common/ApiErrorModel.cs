namespace Poll.Api.Models.Common;

/// <summary>
/// Модель ответа клиенту в случае ошибки.
/// </summary>
public class ApiErrorModel
{
    /// <summary>
    /// Статус-код.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Сообщение.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Идентификатор трейса.
    /// </summary>
    public string TraceIdentifier { get; init; }
}