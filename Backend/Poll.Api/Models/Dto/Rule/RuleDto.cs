using Poll.Core.Entities.ViewRules;

namespace Poll.Api.Models.Dto.Rule;

/// <summary>
/// Объект передачи данных для создания правила.
/// </summary>
public class RuleDto
{
    /// <summary>
    /// Идентификатор вопроса.
    /// </summary>
    public string QuestionId { get; set; }

    /// <summary>
    /// Значение, с которым проводим сравнение.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Тип сравнения.
    /// </summary>
    public ConditionType Type { get; set; }
}