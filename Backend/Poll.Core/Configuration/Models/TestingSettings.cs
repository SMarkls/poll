#if DEBUG
namespace Poll.Core.Configuration.Models;
public class TestingSettings
{
    public List<string> Users { get; set; } = [];
    public List<string> Pollers { get; set; } = [];
    public List<string> Admins { get; set; } = [];
}
#endif
