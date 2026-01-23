using Microsoft.AspNetCore.Authorization;

namespace EventAssist.Authorization
{
    public class RequireFullTokenRequirement : IAuthorizationRequirement
    {
    }
}
