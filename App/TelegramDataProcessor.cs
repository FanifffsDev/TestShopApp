using TestShopApp.App.Controllers.Api;
using TestShopApp.Common.Data;

namespace TestShopApp.App;

public class TelegramDataProcessor : ITelegramDataProcessor
{
    public async Task<ExecutionResult> VerifyData(TelegramInitData data)
    {
        return new ExecutionResult(success: false);
    }
}