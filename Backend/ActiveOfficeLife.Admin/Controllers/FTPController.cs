using Microsoft.AspNetCore.Mvc;

namespace ActiveOfficeLife.Admin.Controllers
{
    public class FTPController : BaseController
    {
        public FTPController(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
