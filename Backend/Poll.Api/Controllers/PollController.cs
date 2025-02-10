using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Filters;
using Poll.Api.Models.Dto;
using Poll.Api.Models.Dto.Poll;
using Poll.Core.Entities.Ldap;
using Poll.Core.Interfaces;
using Poll.Core.Services.Authorization;
using Poll.Core.Services.Poll;

namespace Poll.Api.Controllers;

public class PollController : BaseController
{
    private readonly IPollService _service;
    private readonly IMapper _mapper;
    private readonly ILdapService _ldapService;

    public PollController(IPollService service, IMapper mapper, ILdapService ldapService)
    {
        _service = service;
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
        return Ok(await _service.AddPoll(entity, Ct));
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
        var entity = await _service.Get(pollId, Ct);
        if (entity is null)
        {
            return BadRequest("Опрос не найден");
        }

        if (entity.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь автором опроса.");
        }

        await _service.Delete(pollId, Ct);
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
        var entity = await _service.Get(pollId, Ct);
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
            entity.EmployeeIds.Count != 0 && !entity.EmployeeIds.Contains(user.Department))
        {
            return BadRequest("Вам недоступен этот опрос");
        }

        var dto = _mapper.Map<GetPollDto>(entity);
        return Ok(dto);
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
        var entity = await _service.Get(pollId, Ct);
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
        var polls = await _service.GetAll(userId, Ct);
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
        var storedEntity = await _service.Get(dto.PollId, Ct);
        if (storedEntity is null)
        {
            return BadRequest("Опрос не найден");
        }

        if (storedEntity.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь автором опроса.");
        }

        var entity = _mapper.Map<Core.Entities.Poll>(dto);
        await _service.Update(entity, Ct);
        return NoContent();
    }
}