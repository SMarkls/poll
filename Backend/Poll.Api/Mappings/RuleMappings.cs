using AutoMapper;
using Poll.Api.Models.Dto.Rule;
using Poll.Core.Entities.ViewRules;

namespace Poll.Api.Mappings;

public class RuleMappings : Profile
{
    public RuleMappings()
    {
        CreateMap<RuleDto, Rule>();

        CreateMap<ViewRuleDto, ViewRule>();
    }
}