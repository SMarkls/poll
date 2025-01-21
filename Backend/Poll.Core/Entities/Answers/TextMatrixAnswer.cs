namespace Poll.Core.Entities.Answers;

public class TextMatrixAnswer : QuestionAnswer
{
    public List<List<string>> TextMatrix { get; init; }
}