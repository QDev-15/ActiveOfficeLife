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
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? pageIndex = 1, int? pageSize = 10)
        {
            // Validate the page index and size
            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            if (pageSize == null)
            {
                pageSize = 10;
            }
            // Validate the date range
            if (startDate == null || endDate == null)
            {
                var logs = await _logRepository.GetAllAsync(pageIndex.Value, pageSize.Value);
                return Ok(new ResultSuccess(new
                {
                    Items = logs.Items,
                    TotalCount = logs.totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }));
            }    
            if (startDate > endDate)
            {
                return BadRequest(new ResultError("Start date cannot be after end date."));
            }
            // Fetch logs based on the date range
            var logsByDate = await _logRepository.GetAllAsync(startDate.Value, endDate.Value, pageIndex.Value, pageSize.Value);
            if (logsByDate.Items == null || !logsByDate.Items.Any())
            {
                logsByDate = (new List<LogModel>(), 0);
                return Ok(new ResultSuccess(new
                {
                    Items = logsByDate.Items,
                    TotalCount = logsByDate.totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }));
            }
            return Ok(new ResultSuccess(new
            {
                Items = logsByDate.Items,
                TotalCount = logsByDate.totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            }));
        }
    }
}
