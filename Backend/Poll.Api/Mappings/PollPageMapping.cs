using AutoMapper;
using Poll.Api.Models.Dto.Poll;
using Poll.Api.Models.Dto.PollPage;
using Poll.Core.Entities;

namespace Poll.Api.Mappings;

public class PollPageMapping : Profile
{
    public PollPageMapping()
    {
        CreateMap<PollPage, GetPollPageDto>();

        CreateMap<AddPollPageDto, PollPage>();

        CreateMap<PollPage, ResultPageDto>()
            .ForMember(x => x.Results, opt => opt.MapFrom(x => x.Questions));
    }	
}