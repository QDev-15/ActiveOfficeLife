using ActiveOfficeLife.Admin.Interfaces;
using ActiveOfficeLife.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class AdController : BaseController
    {
        private readonly IApiService _apiService;
        public AdController(IConfiguration configuration) : base(configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateOrUpdate(AdModel adModel)
        {
            return View();
        }
    }
}
