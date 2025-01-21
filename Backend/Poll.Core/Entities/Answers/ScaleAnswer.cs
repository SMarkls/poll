namespace Poll.Core.Entities.Answers;

/// <summary>
/// Ответ на вопрос типа "Шкала".
/// </summary>
public class ScaleAnswer : QuestionAnswer
{
    public int Value { get; init; }
}