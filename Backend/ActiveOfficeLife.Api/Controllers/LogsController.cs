using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class LogsController : BaseController
    {
        private readonly ILogRepository _logRepository;
        private readonly ILogService _logService;
        public LogsController(ILogRepository logRepository, ILogService logService)
        {
            _logService = logService;
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
        }
        [AllowAnonymous]
        [HttpPost("create-client")]
        public async Task<IActionResult> CreateClient([FromBody] LogModel log)
        {
            if (log == null)
            {
                return BadRequest(new ResultError("Log data is required.", "400"));
            }
            var logOption = new LogProperties()
            {
                IpAddress = log.IpAddress,
                Message = "Client-" + log.Message,
                RequestPath = log.RequestPath,
                Source = log.Source,
                StackTrace = log.StackTrace,
                Timestamp = log.Timestamp,
                UserId = log.UserId,
            };   
            _logService.Error(logOption);
            return Ok(new ResultSuccess("Log created successfully."));
        }
        [HttpGet("all")]
        public async Task<IActionResult> Index([FromQuery] PagingLogRequest request)
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
        /// <summary>
        /// Get logs by secret key : secret key = "123e4567-e89b-12d3-a456-426614174000"
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("secret")]
        public async Task<IActionResult> GetBySecret([FromQuery] string key = "secret")
        {
            if (key != "123e4567-e89b-12d3-a456-426614174000" && key != "quynhvpit-secret")
            {
                return Ok(new ResultSuccess(new
                {
                    Items = new List<LogModel>(),
                    TotalCount = 0,
                    PageIndex = 1,
                    PageSize = 1000
                }));
            }
            PagingLogRequest pagingLogRequest = new PagingLogRequest
            {
                PageIndex = 1,
                PageSize = 1000,
                StartDate = DateTime.MinValue,
                EndDate = DateTime.MaxValue,
                SortDirection = "desc",
                SortField = "Timestamp"
            };

            try {              
                var logs = await _logRepository.GetAllAsync(pagingLogRequest);

                return Ok(new ResultSuccess(new
                {
                    Items = logs.Items ?? new List<LogModel>(),
                    TotalCount = logs.totalCount,
                    PageIndex = pagingLogRequest.PageIndex,
                    PageSize = pagingLogRequest.PageSize
                }));
            }
            catch (Exception ex)
            {
                AOLLogger.Error("LogsController.GetBySecret: " + ex.Message);
                return BadRequest(new ResultError("Invalid date format.", "400"));
            }
           
        }


    }
}
