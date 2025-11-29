using Microsoft.AspNetCore.Mvc;

namespace TestShopApp.Api.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("{path:regex(^(?!api).*$)}")]
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/index.html"), "text/html");
        }
    }
}
