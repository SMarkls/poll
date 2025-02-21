using Poll.Core.Entities.Enums;
using Poll.Core.Entities.Variants;

namespace Poll.Api.Models.Dto.Question;

/// <summary>
/// Объект передачи данных создания вопроса.
/// </summary>
public class QuestionDto
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
    /// Варианты ответа.
    /// </summary>
    public BaseVariants Variants { get; init; }
}