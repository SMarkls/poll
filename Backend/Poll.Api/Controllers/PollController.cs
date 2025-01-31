using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Models.Common;
using Poll.Api.Models.Dto;
using Poll.Api.Models.Dto.Poll;
using Poll.Core.Services.Poll;

namespace Poll.Api.Controllers;

public class PollController : BaseController
{
    private readonly IPollService _service;
    private readonly IMapper _mapper;

    public PollController(IPollService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [ProducesResponseType<string>(StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> Add(AddPollDto dto)
    {
        dto.OwnerId = CurrentUser.Id;
        var entity = _mapper.Map<Core.Entities.Poll>(dto);
        return Ok(await _service.AddPoll(entity, HttpContext.RequestAborted));
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(string pollId)
    {
        var entity = await _service.Get(pollId, HttpContext.RequestAborted);
        if (entity is null)
        {
            return BadRequest("Опрос не найден");
        }

        if (entity.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь автором опроса.");
        }

        await _service.Delete(pollId, HttpContext.RequestAborted);
        return NoContent();
    }

    [HttpGet("{pollId}")]
    [ProducesResponseType<GetPollDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(string pollId) 
    {
        var entity = await _service.Get(pollId, HttpContext.RequestAborted);
        if (entity is null)
        {
            return BadRequest("Опрос не найден");
        }

        if (entity.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь автором опроса.");
                    
        }

        var dto = _mapper.Map<GetPollDto>(entity);
        return Ok(dto);
    }

    [HttpGet("result/{pollId}")]
    [ProducesResponseType<ResultDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetResult(string pollId)
    {
        var entity = await _service.Get(pollId, HttpContext.RequestAborted);
        if (entity?.OwnerId == CurrentUser.Id)
        {
            return Ok(_mapper.Map<ResultDto>(entity));
        }

        return BadRequest("Вы не являетесь создателем опроса");
    }

    [HttpGet]
    [ProducesResponseType<List<GetAllPollsDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var userId = CurrentUser.Id; 
        var polls = await _service.GetAll(userId, HttpContext.RequestAborted);
        var pollsDto = _mapper.ProjectTo<GetAllPollsDto>(polls.AsQueryable()).ToList();
        return Ok(pollsDto);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(UpdatePollDto dto)
    {
        var storedEntity = await _service.Get(dto.PollId, HttpContext.RequestAborted);
        if (storedEntity is null)
        {
            return BadRequest("Опрос не найден");
        }

        if (storedEntity.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь автором опроса.");
        }

        var entity = _mapper.Map<Core.Entities.Poll>(dto);
        await _service.Update(entity, HttpContext.RequestAborted);
        return NoContent();
    }
}