using Microsoft.AspNetCore.Authorization;

namespace EventAssist.Authorization
{
    public class RequireFullTokenHandler : AuthorizationHandler<RequireFullTokenRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            RequireFullTokenRequirement requirement)
        {
            var isTempTokenClaim = context.User.FindFirst("isTempToken");

            if (isTempTokenClaim == null || isTempTokenClaim.Value == "True")
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
