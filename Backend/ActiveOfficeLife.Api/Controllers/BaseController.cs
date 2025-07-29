using ActiveOfficeLife.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ActiveOfficeLife.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        protected string IpAddress { get; private set; } = "Unknown";
        protected Guid UserId { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IpAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out Guid parsedUserId))
            {
                UserId = parsedUserId;
            }
            else
            {
                UserId = Guid.NewGuid();
            }
            base.OnActionExecuting(context);
        }
    }
}
