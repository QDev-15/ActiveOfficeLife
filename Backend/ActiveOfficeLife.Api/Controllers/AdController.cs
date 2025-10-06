using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Application.Services;
using ActiveOfficeLife.Common;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Common.Requests;
using ActiveOfficeLife.Common.Responses;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Api.Controllers
{
    public class AdController : BaseController
    {
        private readonly IAdService _adService;
        private readonly CustomMemoryCache _cache;
        private readonly AppConfigService _appConfigService;
        private string _controllerName = "AdController";

        public AdController(IAdService adService, CustomMemoryCache cache, AppConfigService appConfigService)
        {
            _adService = adService;
            _cache = cache;
            _appConfigService = appConfigService;
        }

        [HttpGet(AOLEndPoint.AdGetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PagingAdRequest pagingAdRequest)
        {
            try
            {
                if (pagingAdRequest == null)
                {
                    pagingAdRequest = new PagingAdRequest();
                }
                pagingAdRequest.Type = pagingAdRequest.Type ?? Common.Enums.AdType.None; // Example of setting a default type if needed
                var returnResult = new ResultSuccess();                                                                    // check cache first
                var cacheKey = $"{_controllerName}-{MethodBase.GetCurrentMethod().Name}-{pagingAdRequest.SortField}-{pagingAdRequest.SortDirection}-{pagingAdRequest.PageIndex}-{pagingAdRequest.PageSize}-{pagingAdRequest.Type}";
                var cachedResult = _cache.Get<(List<AdModel> Items, int Count)>(cacheKey);
                if (cachedResult.Items != null && cachedResult.Items.Any())
                {
                    returnResult = new ResultSuccess(new
                    {
                        Items = cachedResult.Items,
                        TotalCount = cachedResult.Count,
                        PageIndex = pagingAdRequest.PageIndex,
                        PageSize = pagingAdRequest.PageSize,
                    });
                    return Ok(returnResult);
                }

                var result = await _adService.GetAllAdsAsync(pagingAdRequest);
                returnResult = new ResultSuccess(new
                {
                    Items = result.Items,
                    TotalCount = result.Count,
                    PageIndex = pagingAdRequest.PageIndex,
                    PageSize = pagingAdRequest.PageSize,
                });
                // Cache the result
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 5 minutes
                return Ok(returnResult);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultError(ex.Message));
            }
        }
        [HttpGet(AOLEndPoint.AdGetById)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                string cacheKey = $"{_controllerName}-{MethodBase.GetCurrentMethod().Name}-{id}";   
                var cachedAd = _cache.Get<AdModel>(cacheKey);
                if (cachedAd != null)
                {
                    return Ok(new ResultSuccess(cachedAd));
                }
                var ad = await _adService.GetAdByIdAsync(id);
                if (ad == null)
                {
                    return NotFound(new ResultError("Ad not found", "404"));
                }
                _cache.Set(cacheKey, ad, TimeSpan.FromMinutes(_appConfigService.AppConfigs.CacheTimeout)); // Cache for 5 minutes
                return Ok(new ResultSuccess(ad));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultError(ex.Message));
            }
        }
        [HttpPost(AOLEndPoint.AdCreate)]
        public async Task<IActionResult> Create(AdModel adModel)
        {
            try
            {
                var createdAd = await _adService.CreateAdAsync(adModel);
                // remove all cache related to ad
                _cache.RemoveByPattern($"{_controllerName}");
                return Ok(new ResultSuccess(createdAd));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultError(ex.Message));
            }
        }
        [HttpPut(AOLEndPoint.AdUpdate)]
        public async Task<IActionResult> Update(AdModel adModel)
        {
            try
            {
                // validate adModel
                if (adModel == null || adModel.Id == Guid.Empty)
                {
                    return BadRequest(new ResultError("Invalid ad data.", "400"));
                }
                var updatedAd = await _adService.UpdateAdAsync(adModel);
                // remove all cache related to ad
                _cache.RemoveByPattern($"{_controllerName}");
                return Ok(new ResultSuccess(updatedAd));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultError(ex.Message));
            }
        }
        [HttpDelete(AOLEndPoint.AdDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // validate id
                if (id == Guid.Empty)
                {
                    return BadRequest(new ResultError("Invalid ad ID.", "400"));
                }
                var deleted = await _adService.DeleteAdAsync(id);
                if (!deleted)
                {
                    return NotFound(new ResultError("Ad not found.", "404"));
                }
                // remove all cache related to ad
                _cache.RemoveByPattern($"{_controllerName}");
                return Ok(new ResultSuccess("Ad deleted successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultError(ex.Message));
            }
        }
    }
}
