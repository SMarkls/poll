namespace Poll.Api.Models.Common;

public class CurrentUser
{
    public required string Id { get; init; }
    public required string Login { get; init; }
    public required string Role { get; init; }
}