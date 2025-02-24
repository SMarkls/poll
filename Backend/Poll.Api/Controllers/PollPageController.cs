using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Extensions;
using Poll.Api.Filters;
using Poll.Api.Models.Dto.PollPage;
using Poll.Api.Models.Dto.Question;
using Poll.Core.Entities;
using Poll.Core.Entities.Ldap;
using Poll.Core.Interfaces;

namespace Poll.Api.Controllers;

/// <summary>
/// Контроллер страницы опросов.
/// </summary>
[AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
[Route("Poll/{pollId}/[controller]/")]
public class PollPageController : BaseController
{
    private readonly IPollPageRepository _repository;
    private readonly IPollRepository _pollRepository;
    private readonly IMapper _mapper;

    public PollPageController(IPollPageRepository repository, IPollRepository pollRepository, IMapper mapper)
    {
        _repository = repository;
        _pollRepository = pollRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Получить страницу опроса.
    /// </summary>
    /// <param name="pollPageId">Идентификатор страницы опроса.</param>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <returns>Объект передачи данных страницы опроса.</returns>
    [HttpGet("{pollPageId}")]
    [ProducesResponseType<GetPollPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] string pollPageId, [FromRoute] string pollId)
    {
        await this.ValidateAccess(pollId, _pollRepository, Ct);
        var entity = await _repository.GetPollPage(pollId, pollPageId, Ct);
        if (entity is null)
        {
            return Error("Страница опроса не найдена", 400);
        }

        var dto = _mapper.Map<GetPollPageDto>(entity);
        return Ok(dto);
    }

    /// <summary>
    /// Добавить страницу опроса.
    /// </summary>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <param name="dto">Объект передачи данных добавления страницы опроса.</param>
    /// <returns>Идентификатор новой страницы опроса.</returns>
    [HttpPut]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Add([FromRoute] string pollId, [FromBody] AddPollPageDto dto)
    {
        await this.ValidateAccess(pollId, _pollRepository, Ct);
        var entity = _mapper.Map<PollPage>(dto);
        var result = await _repository.AddPollPage(pollId, entity, Ct);
        return Ok(result);
    }

    /// <summary>
    /// Удалить страницу опроса.
    /// </summary>
    /// <param name="pollPageId">Идентификатор страницы опроса.</param>
    /// <param name="pollId">Идентификатор опроса.</param>
    [HttpDelete("{pollPageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromRoute] string pollPageId, [FromRoute] string pollId)
    {
        await this.ValidateAccess(pollId, _pollRepository, Ct);
        await _repository.RemovePollPage(pollId, pollPageId, Ct);
        return NoContent();
    }

    /// <summary>
    /// Обновить заголовок страницы опроса.
    /// </summary>
    /// <param name="header">Новый заголовок.</param>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <param name="pollPageId">Идентификатор страницы опроса.</param>
    [HttpPatch("{pollPageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateHeader([FromBody] string header, [FromRoute] string pollId,
        [FromRoute] string pollPageId)
    {
        await this.ValidateAccess(pollId, _pollRepository, Ct);

        await _repository.UpdateHeader(pollId, pollPageId, header, Ct);
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
    public async Task<IActionResult> CreateQuestion([FromRoute] string pollPageId, [FromRoute] string pollId,
        [FromBody] QuestionDto dto)
    {
        await this.ValidateAccess(pollId, _pollRepository, Ct);

        var question = _mapper.Map<Question>(dto);
        return Ok(await _repository.AddQuestion(pollId, pollPageId, question, Ct));
    }

    /// <summary>
    /// Удалить вопрос.
    /// </summary>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <param name="pollPageId">Идентификатор страницы.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <returns></returns>
    [HttpDelete("{pollPageId}/question/{questionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteQuestion([FromRoute] string pollId, [FromRoute] string pollPageId, 
        [FromRoute] string questionId)
    {
        await this.ValidateAccess(pollId, _pollRepository, Ct);

        await _repository.DeleteQuestion(pollId, pollPageId, questionId, Ct);
        return NoContent();
    }

    /// <summary>
    /// Изменить текст вопроса.
    /// </summary>
    /// <param name="questionId">Идентификатор вопроса.</param>
    /// <param name="pollPageId">Идентификатор страницы опроса.</param>
    /// <param name="dto">Объект передачи данных вопроса.</param>
    /// <param name="pollId">Идентификатор опроса.</param>
    [HttpPut("{pollPageId}/question/{questionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EditQuestionText([FromRoute] string pollId, [FromRoute] string questionId, 
        [FromRoute] string pollPageId, [FromBody] QuestionDto dto)
    {
        await this.ValidateAccess(pollId, _pollRepository, Ct);

        var entity = _mapper.Map<Question>(dto);
        await _repository.UpdateQuestion(pollId, pollPageId, questionId, entity, Ct);
        return NoContent();
    }
}