using TestShopApp.App.Controllers.Api;
using TestShopApp.Common.Data;

namespace TestShopApp.App;

public interface ITelegramDataProcessor
{
    public Task<ExecutionResult> VerifyData(TelegramInitData data);
}