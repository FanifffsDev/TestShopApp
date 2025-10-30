using Microsoft.AspNetCore.Mvc;

namespace TestShopApp.App.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("/home")]
        public IActionResult Handle()
        {
            var method = Request.Method;
            var path = Request.Path;
            var query = Request.QueryString.Value;

            return Ok(new
            {
                Message = "Обработано универсальным контроллером",
                Method = method,
                Path = path,
                Query = query
            });
        }

        [Route("/home/init")]
        public IActionResult HandleInit([FromBody] TelegramInitData data)
        {
            var method = Request.Method;
            var path = Request.Path;
            var query = Request.QueryString.Value;

            return Ok(new
            {
                Message = "Обработано универсальным контроллером",
                Method = method,
                Path = path,
                Query = query
            });
        }
    }

    public class TelegramInitData
    {
        public TelegramUser? user { get; set; }
        public string? query_id { get; set; }
        public string? auth_date { get; set; }
        public string? hash { get; set; }
    }

    public class TelegramUser
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? username { get; set; }
        public string? language_code { get; set; }
    }
}
