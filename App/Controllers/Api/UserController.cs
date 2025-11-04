using Microsoft.AspNetCore.Mvc;
using TestShopApp.Common.Repo;

namespace TestShopApp.App.Controllers.Api;

[ApiController]
[Route("api/v1/users")]
public class UserController(ITgUserRepo userRepo, ITelegramDataProcessor initDataVerifier) : ControllerBase
{
    private readonly ITgUserRepo _userRepo = userRepo;
    private readonly ITelegramDataProcessor _initDataVerifier = initDataVerifier;
    
    [Route("/bake")]
    [HttpPost]
    public IActionResult Bake([FromBody] TelegramInitData initData)
    {
        var method = Request.Method;
        var path = Request.Path;
        var query = Request.QueryString.Value;
        
        //var res = _initDataVerifier.VerifyData(initData);
        

        return Ok(new
        {
            Message = initData,
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