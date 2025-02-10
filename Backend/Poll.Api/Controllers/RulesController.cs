using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Filters;
using Poll.Api.Models.Dto.Rule;
using Poll.Core.Entities.Ldap;
using Poll.Core.Entities.ViewRules;
using Poll.Core.Services.ViewRule;

namespace Poll.Api.Controllers;

public record FullQuestionArgs(string PollId, string PollPageId, string QuestionId);

[AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
public class RulesController : BaseController
{
    private readonly IRuleService _service;
    private readonly IMapper _mapper;

    public RulesController(IRuleService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    /// <summary>
    /// Добавить или обновить вопрос.
    /// </summary>
    /// <param name="rule">Объект передачи данных добавления/удаления правила.</param>
    /// <param name="args">Координаты вопроса.</param>
    /// <returns>Идентификатор вопроса.</returns>
    [HttpPut]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrUpdate(ViewRuleDto rule, [FromQuery] FullQuestionArgs args)
    {
        var entity = _mapper.Map<ViewRule>(rule);
        var result = await _service.CreateOrUpdateRule(entity, args.PollId, args.PollPageId, args.QuestionId, Ct);
        return Ok(result);
    }

    /// <summary>
    /// Удалить правило.
    /// </summary>
    /// <param name="args">Координаты вопроса.</param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromQuery] FullQuestionArgs args)
    {
        await _service.DeleteRule(args.PollId, args.PollPageId, args.QuestionId, Ct);
        return Ok();
    }
}