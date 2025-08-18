using ActiveOfficeLife.Common.Models;
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
        public async Task<IActionResult> Index(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            // Gán mặc định nếu không có giá trị
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            // Validate khoảng ngày
            if (startDate > endDate)
            {
                return BadRequest(new ResultError("Start date cannot be after end date.", "400"));
            }

            // Lấy dữ liệu
            var logs = await _logRepository.GetAllAsync(startDate.Value, endDate.Value, pageIndex, pageSize);

            return Ok(new ResultSuccess(new
            {
                Items = logs.Items ?? new List<LogModel>(),
                TotalCount = logs.totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            }));
        }

    }
}
