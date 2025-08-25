using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class LogerController : BaseController
    {
        private readonly ILogRepository _logRepository;
        public LogerController(ILogRepository logRepository)
        {
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
        }
        [HttpGet("all")]
        public async Task<IActionResult> Index([FromQuery] PagingRequest request)
        {
            // Gán mặc định nếu không có giá trị
            request.StartDate ??= DateTime.MinValue;
            request.EndDate ??= DateTime.MaxValue;

            // Validate khoảng ngày
            if (request.StartDate > request.EndDate)
            {
                return BadRequest(new ResultError("Start date cannot be after end date.", "400"));
            }

            // Lấy dữ liệu
            var logs = await _logRepository.GetAllAsync(request);

            return Ok(new ResultSuccess(new
            {
                Items = logs.Items ?? new List<LogModel>(),
                TotalCount = logs.totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            }));
        }

    }
}
