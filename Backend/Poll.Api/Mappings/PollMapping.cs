using AutoMapper;
using Poll.Api.Models.Dto;
using Poll.Api.Models.Dto.Poll;

namespace Poll.Api.Mappings;

public class PollMapping : Profile
{
    public PollMapping()
    {
        CreateMap<AddPollDto, Core.Entities.Poll>();

        CreateMap<Core.Entities.Poll, GetPollDto>();

        CreateMap<Core.Entities.Poll, GetAllPollsDto>();

        CreateMap<Core.Entities.Poll, ResultDto>()
            .ForMember(x => x.PollId, opt => opt.MapFrom(src => src.PollId.ToString()))
            .ForMember(x => x.Pages, opts => opts.MapFrom(src => src.Pages));
    }
}