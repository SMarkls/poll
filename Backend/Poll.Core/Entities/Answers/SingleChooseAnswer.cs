namespace Poll.Core.Entities.Answers;

/// <summary>
/// Ответ на вопрос типа "Одиночный выбор".
/// </summary>
public class SingleChooseAnswer : QuestionAnswer
{
    /// <summary>
    /// Номер ответа.
    /// </summary>
    public int ChoiceNumber { get; set; }
}