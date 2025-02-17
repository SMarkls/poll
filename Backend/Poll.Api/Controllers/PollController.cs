using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Extensions;
using Poll.Api.Filters;
using Poll.Api.Models.Dto.Poll;
using Poll.Core.Entities.Answers;
using Poll.Core.Entities.Ldap;
using Poll.Core.Interfaces;

namespace Poll.Api.Controllers;

/// <summary>
/// Контроллер сущности опрос.
/// </summary>
public class PollController : BaseController
{
    private readonly IPollRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILdapService _ldapService;

    public PollController(IPollRepository repository, IMapper mapper, ILdapService ldapService)
    {
        _repository = repository;
        _mapper = mapper;
        _ldapService = ldapService;
    }

    /// <summary>
    /// Добавить опрос.
    /// </summary>
    /// <param name="dto">Объект передачи данных для добавления опроса.</param>
    /// <returns>Идентификатор нового опроса.</returns>
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [HttpPost]
    [AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
    public async Task<IActionResult> Add(AddPollDto dto)
    {
        dto.OwnerId = CurrentUser.Id;
        var entity = _mapper.Map<Core.Entities.Poll>(dto);
        return Ok(await _repository.Add(entity, Ct));
    }

    /// <summary>
    /// Удалить опрос.
    /// </summary>
    /// <param name="pollId">Идентификатор опроса.</param>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
    public async Task<IActionResult> Delete(string pollId)
    {
        await this.ValidateAccess(pollId, _repository, Ct);

        await _repository.Delete(pollId, Ct);
        return NoContent();
    }

    /// <summary>
    /// Получить опрос.
    /// </summary>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <returns>Объект передачи данных получения опроса.</returns>
    [HttpGet("{pollId}")]
    [ProducesResponseType<GetPollDto>(StatusCodes.Status200OK)]
    [AuthorizedOnly(UserRoles = [])]
    public async Task<IActionResult> Get(string pollId) 
    {
        var entity = await _repository.GetById(pollId, Ct);
        if (entity is null)
        {
            return BadRequest("Опрос не найден");
        }

        var user = await _ldapService.GetFromDb(CurrentUser.Id);
        if (user is null)
        {
            return Unauthorized("Пользователь не найден");
        }

        if (entity.DepartmentIds.Count != 0 && !entity.DepartmentIds.Contains(user.Department) ||
            entity.EmployeeIds.Count != 0 && !entity.EmployeeIds.Contains(user.ObjectGuid))
        {
            return BadRequest("Вам недоступен этот опрос");
        }

        var dto = _mapper.Map<GetPollDto>(entity);
        return Ok(dto);
    }

    [HttpPost("{pollId}")]
    public async Task<IActionResult> CompletePoll(CompletePollDto dto, string pollId)
    {
        await _repository.Complete(pollId, CurrentUser.Id, dto, Ct);
        return NoContent();
    }

    /// <summary>
    /// Получить результат прохождения опроса.
    /// </summary>
    /// <param name="pollId">Идентификатор опроса.</param>
    /// <returns>Объект передачи данных результата опроса.</returns>
    [HttpGet("result/{pollId}")]
    [ProducesResponseType<ResultDto>(StatusCodes.Status200OK)]
    [AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
    public async Task<IActionResult> GetResult(string pollId)
    {
        var entity = await _repository.GetById(pollId, Ct);
        if (entity?.OwnerId == CurrentUser.Id)
        {
            return Ok(_mapper.Map<ResultDto>(entity));
        }

        return BadRequest("Вы не являетесь создателем опроса");
    }

    /// <summary>
    /// Получить все опросы авторизованного пользователя.
    /// </summary>
    /// <returns>Список опросов.</returns>
    [HttpGet]
    [ProducesResponseType<List<GetAllPollsDto>>(StatusCodes.Status200OK)]
    [AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
    public async Task<IActionResult> GetAll()
    {
        var userId = CurrentUser.Id; 
        var polls = await _repository.GetAll(userId, Ct);
        var pollsDto = _mapper.ProjectTo<GetAllPollsDto>(polls.AsQueryable()).ToList();
        return Ok(pollsDto);
    }

    /// <summary>
    /// Обновить опрос.
    /// </summary>
    /// <param name="dto">Объект передачи данных обновления опроса.</param>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AuthorizedOnly(UserRoles = [UserRole.Poller, UserRole.Admin])]
    public async Task<IActionResult> Update(UpdatePollDto dto)
    {
        await this.ValidateAccess(dto.PollId, _repository, Ct);

        var entity = _mapper.Map<Core.Entities.Poll>(dto);
        await _repository.Update(entity, Ct);
        return NoContent();
    }
}