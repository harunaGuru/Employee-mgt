using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace myproject.Security
{
    public class CanEditOtherAdminRoleAndClaimsHandler : AuthorizationHandler<ManageAdminRoleAndClaimRequirement>
    {
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRoleAndClaimRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if(authFilterContext == null)
            {
                return Task.CompletedTask;
            }
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string AdminBeingEdited = authFilterContext.HttpContext.Request.Query["UserId"];
            if(context.User.IsInRole("Admin") && context.User.HasClaim(claim=> claim.Type == "Edit Role" && claim.Value == "true") 
                && loggedInAdminId != AdminBeingEdited)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
