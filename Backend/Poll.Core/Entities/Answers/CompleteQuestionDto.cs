namespace Poll.Core.Entities.Answers;

/// <summary>
/// Объект передачи данных прохождения вопроса.
/// </summary>
public class CompleteQuestionDto
{
    /// <summary>
    /// Идентификатор вопроса.
    /// </summary>
    public string QuestionId { get; set; }

    /// <summary>
    /// Значение ответа на вопрос.
    /// </summary>
    public string Value { get; set; }
}