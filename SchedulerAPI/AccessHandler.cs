using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchedulerAPI
{
    public class AccessHandler : AuthorizationHandler<AccessRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRequirement requirement)
        {
            // check if user is an admin or the resource owner
            if (context.User.IsInRole("Admin") || requirement.ResourceOwner == context.User.FindFirstValue(ClaimTypes.NameIdentifier)) ;
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
