using Poll.Api.Controllers;
using Poll.Core.Exceptions;
using Poll.Core.Interfaces;

namespace Poll.Api.Extensions;

public static class ControllerExtensions
{
    public static async Task ValidateAccess(this BaseController ctler, string pollId, IPollRepository pollService,
        CancellationToken ct)
    {
        var ownerId = await pollService.GetOwnerId(pollId, ct);
        if (ownerId != ctler.CurrentUser.Id)
        {
            throw new PermissionDeniedException();
        }
    }
}