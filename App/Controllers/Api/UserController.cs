using Microsoft.AspNetCore.Mvc;
using TestShopApp.Common.Repo;
using TestShopApp.Telegram.Utils;

namespace TestShopApp.App.Controllers.Api;

[ApiController]
[Route("api/v1/users")]
public class UserController(ITgUserRepo userRepo, ITelegramDataProcessor initDataVerifier) : ControllerBase
{
    private readonly ITgUserRepo _userRepo = userRepo;
    private readonly ITelegramDataProcessor _initDataVerifier = initDataVerifier;
    
    [Route("bake")]
    [HttpPost]
    public IActionResult Bake([FromBody] string initData)
    {
        var isVerified = TgAuthUtils.VerifyInitData(initData);

        return Ok(new
        {
            Message = isVerified ? "Good data" : "Bad data"
        });
    }
}

public class TelegramInitDataWrapper
{
    public TelegramInitData? initData { get; set; }
}

public class TelegramInitData
{
    public TelegramUser? user { get; set; }
    public string? query_id { get; set; }
    public string? auth_date { get; set; }
    public string? hash { get; set; }
    public string? signature { get; set; }
}

public class TelegramUser
{
    public bool allows_write_to_pm { get; set; }
    public long id { get; set; }
    public bool is_premium { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public string? username { get; set; }
    public string? photo_url { get; set; }
    public string? language_code { get; set; }
}