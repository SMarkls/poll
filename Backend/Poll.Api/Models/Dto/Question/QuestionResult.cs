using Poll.Core.Entities.Answers;
using Poll.Core.Entities.Enums;
using Poll.Core.Entities.Variants;

namespace Poll.Api.Models.Dto.Question;

public class QuestionResult
{
    /// <summary>
    /// Текст вопроса.
    /// </summary>
    public string QuestionText { get; init; }

    /// <summary>
    /// Тип вопроса.
    /// </summary>
    public QuestionType QuestionType { get; init; }

    /// <summary>
    /// Ответы на вопрос.
    /// </summary> 
    public List<QuestionAnswer> Answers { get; init; } = [];

    /// <summary>
    /// Варианты ответа.
    /// </summary>
    public BaseVariants Variants { get; init; }
}
