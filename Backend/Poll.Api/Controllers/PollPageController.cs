using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Models.Dto.PollPage;
using Poll.Core.Entities;
using Poll.Core.Services.Poll;
using Poll.Core.Services.PollPage;

namespace Poll.Api.Controllers;

public record PollArgs(string PollPageId, string PollId);

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

    [HttpGet]
    [ProducesResponseType<GetPollPageDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] PollArgs args)
    {
        var entity = await _service.GetPollPage(args.PollPageId, args.PollId);
        if (entity is null)
        {
            return BadRequest("Опрос не найден");
        }

        var dto = _mapper.Map<GetPollPageDto>(entity);
        return Ok(dto);
    }

    [HttpPut]
    [ProducesResponseType<string>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Add(AddPollPageDto dto)
    {
        var entity = _mapper.Map<PollPage>(dto);
        var poll = await _pollService.Get(dto.PollId);
        if (poll?.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь создателем опроса.");
        }

        var result = await _service.AddPollPage(entity, dto.PollId);
        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromQuery] PollArgs args)
    {
        await _service.RemovePollPage(args.PollPageId, args.PollId);
        return NoContent();
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateHeader([FromQuery] PollArgs args, [FromBody] string header)
    {
        await _service.UpdateHeader(args.PollPageId, args.PollId, header);
        return NoContent();
    }

    [HttpDelete("question/{id}")]
    public async Task<IActionResult> DeleteQuestion([FromRoute] int id)
    {
        _service.DeleteQuestion();
    }
}