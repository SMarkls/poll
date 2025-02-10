using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Poll.Core.Entities.ViewRules;

public class ViewRule
{
    [BsonId]
    public ObjectId RuleId { get; set; }
    public ViewEffect Effect { get; set; }
    public List<List<Rule>> Rules { get; set; }
}