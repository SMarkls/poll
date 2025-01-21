namespace Poll.Core.Entities.Answers;

/// <summary>
/// Сущность ответа на вопрос типа "Матрица".
/// </summary>
public class MatrixAnswer : QuestionAnswer
{
    public List<List<int>> Answers { get; init; }
}