namespace Poll.Core.Entities.Answers;

/// <summary>
/// Сущность ответа вопрос типа "Множественный выбор".
/// </summary>
public class MultiChoiceAnswer : QuestionAnswer
{
    public List<int> Choices { get; init; }
}