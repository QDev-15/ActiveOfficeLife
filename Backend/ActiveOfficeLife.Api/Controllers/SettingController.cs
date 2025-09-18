using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Responses;
using ActiveOfficeLife.Domain.Interfaces;
using Google.Apis.Auth.OAuth2;
using GoogleApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class SettingController : BaseController
    {
        private readonly string _controllerName = "Setting";
        private readonly ISettingService _settingService;
        private readonly IGoogleDriveInterface _googleDriveService;
        private readonly AppConfigService _appConfigService;
        private readonly CustomMemoryCache _cache;
        public SettingController(ISettingService settingService, CustomMemoryCache customMemoryCache, AppConfigService appConfigService, IGoogleDriveInterface googleDriveInterface)
        {
            _cache = customMemoryCache ?? throw new ArgumentNullException(nameof(customMemoryCache));
            _googleDriveService = googleDriveInterface;
            _appConfigService = appConfigService;
            _settingService = settingService;
            _controllerName = this.GetType().Name;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get(string? id)
        {
            string cacheKey = $"{_controllerName}-{MethodBase.GetCurrentMethod().Name}-{id}";
            var cachedResult = _cache.Get<SettingModel>(cacheKey);
            if (cachedResult != null)
            {
                return Ok(new ResultSuccess(cachedResult));
            }
            var setting = await _settingService.GetDefault(id);
            _cache.Set(cacheKey, setting, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout));
            if (setting == null)
            {
                return NotFound(new ResultError("Setting not found", "404"));
            }
            return Ok(new ResultSuccess(setting));
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] SettingModel setting)
        {
            if (setting == null)
            {
                return BadRequest(new ResultError("Invalid setting data", "400"));
            }
            var createdSetting = await _settingService.Create(setting);
            _cache.RemoveByPattern($"{_controllerName}"); // Xoá cache danh sách để cập nhật
            return Ok(new ResultSuccess(createdSetting));
        }
        [HttpPatch("patch")]
        public async Task<IActionResult> Patch([FromBody] JsonElement settingPatch)
        {
            
            // Lấy id (case-insensitive). Khuyến nghị: đưa id lên route luôn cho gọn.
            if (!Helper.TryGetGuidCaseInsensitive(settingPatch, "id", out var id) || id == Guid.Empty)
                return BadRequest("Missing id");

            var current = await _settingService.GetById(id);
            if (current == null) return NotFound();

            var opts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,         // serialize theo camelCase
                PropertyNameCaseInsensitive = true,                        // đọc không phân biệt hoa/thường
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            // Serialize entity hiện tại sang JsonObject
            var currentNode = JsonSerializer.SerializeToNode(current, opts)!.AsObject();

            // Parse payload patch sang JsonObject
            var patchNode = JsonNode.Parse(settingPatch.GetRawText())!.AsObject();

            // Không cho đổi Id (nếu có gửi kèm)
            patchNode.Remove("id");

            // Merge (đệ quy cho object con; array thì replace hoàn toàn)
            Helper.MergeJson(currentNode, patchNode);

            // Deserialize ngược về model
            var merged = currentNode.Deserialize<SettingModel>(opts)!;

            // Bảo toàn các field gốc id, name, createdAt,...
            merged.Id = current.Id;
            var updated = await _settingService.Update(merged);
            _cache.RemoveByPattern($"{_controllerName}"); // Xoá cache danh sách để cập nhật
            return Ok(updated);
        }
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] SettingModel setting)
        {
            if (setting == null || setting.Id == System.Guid.Empty)
            {
                return BadRequest(new ResultError("Invalid setting data", "400"));
            }
            try
            {
                var updatedSetting = await _settingService.Update(setting);
                _cache.RemoveByPattern($"{_controllerName}"); // Xoá cache danh sách để cập nhật
                return Ok(new ResultSuccess(updatedSetting));
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ResultError(ex.Message, "400"));
            }
        }
        [AllowAnonymous]
        /// Google Drive API
        [HttpGet("googledrive/connect")]
        public async Task<IActionResult> ConnectGoogleDrive(string settingId)
        {
            if (string.IsNullOrEmpty(settingId))
            {
                return NotFound(new ResultError("Setting not found", "404"));
            }
            var setting = await _settingService.GetDefault(settingId);
            if (setting == null)
            {
                return NotFound(new ResultError("Setting not found", "404"));
            }

            var RedirectUri = _appConfigService.AppConfigs.ApiUrl + "/api/auth/callback";
            // Khi chạy local thì dùng RedirectUriLocal, khi chạy server thì dùng RedirectUriServer
            var url = _googleDriveService.GetAuthorizationUrl(new ClientSecrets()
            {
                ClientId = setting.GoogleClientId,
                ClientSecret = setting.GoogleClientSecretId
            }, RedirectUri, _appConfigService.AppConfigs.ApplicationName, setting.Id.ToString());
            _cache.RemoveByPattern(_controllerName);
            return Redirect(url);
            //return Ok(new ResultSuccess(url));
        }
        [HttpGet("googledrive/disconnect")]
        public async Task<IActionResult> DisconnectGoogleDrive(string settingId)
        {
            var setting = await _settingService.GetDefault(settingId);
            if (setting == null)
            {
                return NotFound(new ResultError("Setting not found", "404"));
            }
            setting.GoogleToken = null;
            await _settingService.Update(setting);
            _cache.RemoveByPattern(_controllerName);
            return Ok(new ResultSuccess("Google Drive disconnected successfully"));
        }
    }
}
