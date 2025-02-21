using MongoDB.Bson;

namespace Poll.Core.Entities.ViewRules;

public class Rule
{
    public ObjectId QuestionId { get; set; }
    public string Value { get; set; }
    public ConditionType Type { get; set; }
}