using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Poll.Api.Models.Dto.PollPage;
using Poll.Core.Entities;
using Poll.Core.Services.Poll;
using Poll.Core.Services.PollPage;

namespace Poll.Api.Controllers;

public record PollArgs(string PollPageId, string PollId);

public record QuestionArgs(string PollId, string QuestionId);

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
        var entity = await _service.GetPollPage(args.PollPageId, args.PollId, HttpContext.RequestAborted);
        if (entity is null)
        {
            return BadRequest("Страница опроса не найдена");
        }

        var dto = _mapper.Map<GetPollPageDto>(entity);
        return Ok(dto);
    }

    [HttpPut]
    [ProducesResponseType<string>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Add(AddPollPageDto dto)
    {
        var entity = _mapper.Map<PollPage>(dto);
        var poll = await _pollService.Get(dto.PollId, HttpContext.RequestAborted);
        if (poll?.OwnerId != CurrentUser.Id)
        {
            return BadRequest("Вы не являетесь создателем опроса.");
        }

        var result = await _service.AddPollPage(entity, dto.PollId, HttpContext.RequestAborted);
        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete([FromQuery] PollArgs args)
    {
        await _service.RemovePollPage(args.PollPageId, args.PollId, HttpContext.RequestAborted);
        return NoContent();
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateHeader([FromQuery] PollArgs args, [FromBody] string header)
    {
        await _service.UpdateHeader(args.PollPageId, args.PollId, header, HttpContext.RequestAborted);
        return NoContent();
    }

    [HttpDelete("{pollPageId}/question")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteQuestion([FromQuery] QuestionArgs args, [FromRoute] string pollPageId)
    {
        await _service.DeleteQuestion(pollPageId, args.PollId, args.QuestionId, HttpContext.RequestAborted);
        return NoContent();
    }

    [HttpPatch("{pollPageId}/question")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EditQuestionText([FromQuery] QuestionArgs args, 
        [FromRoute] string pollPageId, [FromBody] string text)
    {
        await _service.EditQuestionText(pollPageId, args.PollId, args.QuestionId, text, HttpContext.RequestAborted);
        return NoContent();
    }
}