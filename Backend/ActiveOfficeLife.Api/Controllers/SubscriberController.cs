using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common.Responses;
using ActiveOfficeLife.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Api.Controllers
{
    public class SubscriberController : BaseController
    {
        private readonly ISubscriberService _subscriberService;
        public SubscriberController(ISubscriberService subscriberService)
        {
            _subscriberService = subscriberService;
        }
        [AllowAnonymous]
        [HttpPost("add")]
        // POST: api/subscriber/add { "email": "" }
        public async Task<IActionResult> AddSubscriberAsync([FromBody] string email)
        {
            if (email == null)
            {
                return BadRequest(new ResultError("Invalid email", "400"));
            }
            var result = await _subscriberService.AddSubscriberAsync(email);
            return Ok(new ResultSuccess(result));
        }

    }
}
