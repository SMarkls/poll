using Poll.Core.Entities.Enums;
using Poll.Core.Entities.Variants;

namespace Poll.Api.Models.Dto.Question;

public class GetQuestionDto
{
    /// <summary>
    /// Идентификатор вопроса.
    /// </summary>
    public string QuestionId { get; set; }

    /// <summary>
    /// Идентификатор страницы, на которой находится опрос.
    /// </summary>
    public string PageId { get; init; }

    /// <summary>
    /// Текст вопроса.
    /// </summary>
    public string QuestionText { get; init; }

    /// <summary>
    /// Тип вопроса.
    /// </summary>
    public QuestionType QuestionType { get; init; }

    /// <summary>
    /// Варианты ответа.
    /// </summary>
    public BaseVariants Variants { get; init; }
}