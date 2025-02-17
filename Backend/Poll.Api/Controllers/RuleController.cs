using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Filters;
using Poll.Api.Models.Dto.Rule;
using Poll.Core.Entities.Ldap;
using Poll.Core.Entities.ViewRules;
using Poll.Core.Interfaces;

namespace Poll.Api.Controllers;

/// <summary>
/// Контроллер правил отображения.
/// </summary>
[AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
[Route("Poll/{pollId}/PollPage/{pollPageId}/question/{questionId}/[controller]")]
public class RuleController : BaseController
{
    private readonly IRuleRepository _repository;
    private readonly IMapper _mapper;

    public RuleController(IRuleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Добавить или обновить вопрос.
    /// </summary>
    /// <param name="rule">Объект передачи данных добавления/удаления правила.</param>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <param name="pollPageId">Идентификатор страницы опроса.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <returns>Идентификатор вопроса.</returns>
    [HttpPut]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrUpdate(ViewRuleDto rule, 
        [FromRoute] string pollId, [FromRoute] string pollPageId, [FromRoute] string questionId)
    {
        var entity = _mapper.Map<ViewRule>(rule);
        var result = await _repository.CreateOrUpdateRule(entity, pollId, pollPageId, questionId, Ct);
        return Ok(result);
    }

    /// <summary>
    /// Удалить правило.
    /// </summary>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <param name="pollPageId">Идентификатор страницы опроса.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromRoute] string pollId, [FromRoute] string pollPageId, 
        [FromRoute] string questionId)
    {
        await _repository.DeleteRule(pollId, pollPageId, questionId, Ct);
        return Ok();
    }
}