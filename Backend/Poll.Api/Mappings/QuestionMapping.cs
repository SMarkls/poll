using AutoMapper;
using Poll.Api.Models.Dto;
using Poll.Api.Models.Dto.Question;
using Poll.Core.Entities;

namespace Poll.Api.Mappings;

public class QuestionMapping : Profile
{
    public QuestionMapping()
    {
        CreateMap<QuestionDto, Question>();

        CreateMap<Question, GetQuestionDto>();

        CreateMap<Question, QuestionResult>();
    }
}