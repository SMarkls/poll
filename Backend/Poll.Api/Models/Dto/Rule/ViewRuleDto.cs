using Poll.Core.Entities.ViewRules;

namespace Poll.Api.Models.Dto.Rule;

/// <summary>
/// Объект передачи данных создания правила.
/// </summary>
public class ViewRuleDto
{
    /// <summary>
    /// Идентификатор правила.
    /// </summary>
    public string? RuleId { get; set; }

    /// <summary>
    /// Эффект, применяющийся в случае соблюдентя условий. 
    /// </summary>
    public ViewEffect Effect { get; set; }

    /// <summary>
    /// Правила.
    /// </summary>
    public List<List<RuleDto>> Rules { get; set; }
}