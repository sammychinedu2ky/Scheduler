using Microsoft.AspNetCore.Authorization;

namespace SchedulerAPI
{
    public class AccessRequirement : IAuthorizationRequirement
    {
        public string ResourceOwner { get; set; }
        public AccessRequirement(string resourceOwner)
        {
            ResourceOwner = resourceOwner;
        }
    }
}