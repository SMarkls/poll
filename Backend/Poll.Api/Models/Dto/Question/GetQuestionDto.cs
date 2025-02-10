using Poll.Api.Models.Dto.Rule;
using Poll.Core.Entities.Enums;
using Poll.Core.Entities.Variants;

namespace Poll.Api.Models.Dto.Question;

/// <summary>
/// Объект передачи данных получения вопроса.
/// </summary>
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

    /// <summary>
    /// Правило отображения.
    /// </summary>
    public ViewRuleDto? ViewRule { get; init; }
}