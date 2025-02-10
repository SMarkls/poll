using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Filters;
using Poll.Api.Models.Dto.PollPage;
using Poll.Api.Models.Dto.Question;
using Poll.Core.Entities;
using Poll.Core.Entities.Ldap;
using Poll.Core.Services.Poll;
using Poll.Core.Services.PollPage;

namespace Poll.Api.Controllers;

public record PollArgs(string PollPageId, string PollId);

public record QuestionArgs(string PollId, string QuestionId);

[AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
public class PollPageController : BaseController
{
    private readonly IPollPageService _service;
    private readonly IPollService _pollService;
    private readonly IMapper _mapper;

    public PollPageController(IPollPageService service, IPollService pollService, IMapper mapper)
    {
        _service = service;
        _pollService = pollService;
        _mapper = mapper;
    }

    /// <summary>
    /// Получить страницу опроса.
    /// </summary>
    /// <param name="args">Координаты опроса.</param>
    /// <returns>Объект передачи данных страницы опроса.</returns>
    [HttpGet]
    [ProducesResponseType<GetPollPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] PollArgs args)
    {
        var entity = await _service.GetPollPage(args.PollPageId, args.PollId, Ct);
        if (entity is null)
        {
            return BadRequest("Страница опроса не найдена");
        }

        var dto = _mapper.Map<GetPollPageDto>(entity);
        return Ok(dto);
    }

    /// <summary>
    /// Добавить страницу опроса.
    /// </summary>
    /// <param name="dto">Объект передачи данных добавления страницы опроса.</param>
    /// <returns>Идентификатор новой страницы опроса.</returns>
    [HttpPut]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Add(AddPollPageDto dto)
    {
        var entity = _mapper.Map<PollPage>(dto);
        var poll = await _pollService.Get(dto.PollId, Ct);
        if (poll?.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь создателем опроса.");
        }

        var result = await _service.AddPollPage(entity, dto.PollId, Ct);
        return Ok(result);
    }

    /// <summary>
    /// Удалить страницу опроса.
    /// </summary>
    /// <param name="args">Координаты страницы.</param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromQuery] PollArgs args)
    {
        await _service.RemovePollPage(args.PollPageId, args.PollId, Ct);
        return NoContent();
    }

    /// <summary>
    /// Обновить заголовок страницы опроса.
    /// </summary>
    /// <param name="args">Координаты страницы.</param>
    /// <param name="header">Новый заголовок.</param>
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateHeader([FromQuery] PollArgs args, [FromBody] string header)
    {
        await _service.UpdateHeader(args.PollPageId, args.PollId, header, Ct);
        return NoContent();
    }

    /// <summary>
    /// Создать вопрос.
    /// </summary>
    /// <param name="pollPageId">Идентификатор страницы опроса.</param>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <param name="dto">Объект передачи данных создания вопроса.</param>
    /// <returns></returns>
    [HttpPost("{pollPageId}/question")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateQuestion([FromRoute] string pollPageId, [FromQuery] string pollId,
        [FromBody] QuestionDto dto)
    {
        var question = _mapper.Map<Question>(dto);
        return Ok(await _service.AddQuestion(pollPageId, pollId, question, Ct));
    }

    /// <summary>
    /// Удалить вопрос.
    /// </summary>
    /// <param name="args">Координаты вопроса.</param>
    /// <param name="pollPageId">Идентификатор страницы.</param>
    /// <returns></returns>
    [HttpDelete("{pollPageId}/question")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteQuestion([FromQuery] QuestionArgs args, [FromRoute] string pollPageId)
    {
        await _service.DeleteQuestion(pollPageId, args.PollId, args.QuestionId, Ct);
        return NoContent();
    }

    // TODO: ПЕРЕДЕЛАТЬ НА ЦЕЛИКОМ ОБНОВЛЕНИЕ ВОПРОСА.
    /// <summary>
    /// Изменить текст вопроса.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="pollPageId"></param>
    /// <param name="text"></param>
    [HttpPatch("{pollPageId}/question")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EditQuestionText([FromQuery] QuestionArgs args, 
        [FromRoute] string pollPageId, [FromBody] string text)
    {
        await _service.EditQuestionText(pollPageId, args.PollId, args.QuestionId, text, Ct);
        return NoContent();
    }
}