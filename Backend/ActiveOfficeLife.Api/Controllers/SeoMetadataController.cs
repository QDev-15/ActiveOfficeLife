using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class SeoMetadataController : BaseController
    {
        private readonly ISeoMetaDataService _seoMetaDataService;
        private readonly CustomMemoryCache _cache;
        private readonly AppConfigService _appConfigService;
        public SeoMetadataController(ISeoMetaDataService seoMetaDataService, CustomMemoryCache customMemoryCache, AppConfigService appConfigService)
        {
            _seoMetaDataService = seoMetaDataService ?? throw new ArgumentNullException(nameof(seoMetaDataService));
            _cache = customMemoryCache ?? throw new ArgumentNullException(nameof(customMemoryCache));
            _appConfigService = appConfigService ?? throw new ArgumentNullException(nameof(appConfigService));
        }
        [AllowAnonymous]
        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] string id) { 
            try 
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new { Message = "Invalid SEO metadata ID.", Code = "400" });
                }
                
                Guid.TryParse(id, out Guid guid);
                var seoMetadata = await _seoMetaDataService.GetById(guid);
                if (seoMetadata == null)
                {
                    return NotFound(new { Message = "SEO metadata not found.", Code = "404" });
                }
                return Ok(new ResultSuccess(seoMetadata));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error fetching SEO metadata: {ex.Message}", ex);
                return BadRequest(new { Message = "Failed to retrieve SEO metadata.", Code = "400" });
            }
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest(new { Message = "Invalid SEO metadata ID.", Code = "400" });
                }
                Guid.TryParse(id, out Guid guid);
                var result = await _seoMetaDataService.Delete(guid);
                if (!result)
                {
                    return NotFound(new { Message = "SEO metadata not found.", Code = "404" });
                }
                return Ok(new ResultSuccess("SEO metadata deleted successfully."));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error deleting SEO metadata: {ex.Message}", ex);
                return BadRequest(new { Message = "Failed to delete SEO metadata.", Code = "400" });
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrUpdate([FromBody] SeoMetadataModel seoMetaDataModel)
        {
            try
            {
                if (seoMetaDataModel == null)
                {
                    return BadRequest(new { Message = "SEO metadata model cannot be null.", Code = "400" });
                }
                var result = await _seoMetaDataService.Add(seoMetaDataModel);
                return Ok(new ResultSuccess(result));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error creating or updating SEO metadata: {ex.Message}", ex);
                return BadRequest(new { Message = "Failed to create or update SEO metadata.", Code = "400" });
            }
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] SeoMetadataModel seoMetaDataModel)
        {
            try
            {
                if (seoMetaDataModel == null || seoMetaDataModel.Id == Guid.Empty)
                {
                    return BadRequest(new { Message = "Invalid SEO metadata model or ID.", Code = "400" });
                }
                var result = await _seoMetaDataService.Update(seoMetaDataModel);
                return Ok(new ResultSuccess(result));
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error updating SEO metadata: {ex.Message}", ex);
                return BadRequest(new { Message = "Failed to update SEO metadata.", Code = "400" });
            }
        }

        // model is a json patch document
        [HttpPatch("patch")]
        public async Task<IActionResult> Patch([FromBody] JsonElement jsonPath)
        {
            try {
                // Lấy id (case-insensitive). Khuyến nghị: đưa id lên route luôn cho gọn.
                if (!Helper.TryGetGuidCaseInsensitive(jsonPath, "id", out var id) || id == Guid.Empty)
                    return BadRequest("Missing id");

                var current = await _seoMetaDataService.GetById(id);
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
                var patchNode = JsonNode.Parse(jsonPath.GetRawText())!.AsObject();

                // Không cho đổi Id (nếu có gửi kèm)
                patchNode.Remove("id");

                // Merge (đệ quy cho object con; array thì replace hoàn toàn)
                Helper.MergeJson(currentNode, patchNode);

                // Deserialize ngược về model
                var merged = currentNode.Deserialize<SeoMetadataModel>(opts)!;

                // Bảo toàn các field gốc id, name, createdAt,...
                merged.Id = current.Id;
                var updated = await _seoMetaDataService.Update(merged);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                AOLLogger.Error($"Error patching SEO metadata: {ex.Message}", ex);
                return BadRequest(new { Message = "Failed to patch SEO metadata.", Code = "400" });
            }
        }

    }
}
