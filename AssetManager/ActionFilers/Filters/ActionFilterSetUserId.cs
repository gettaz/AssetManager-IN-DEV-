using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ActionFilters.Filters
{
    public class ActionFilterSetUserId : IActionFilter
    {

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if ((bool)(httpContext?.User.Identity.IsAuthenticated))
            {
                if (userId != null)
                {
                    context.HttpContext.Items["UserId"] = userId;
                }
            }
        }

    }
}
