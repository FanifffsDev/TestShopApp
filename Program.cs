using Telegram.Bot;
using TestShopApp;
using TestShopApp.Common.Repo;
using TestShopApp.Telegram.Commands;
using TestShopApp.Telegram.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var token = "8344409652:AAHcBXyAMfqsZxZ3M5tSqlIKjKw6IEfLJ8g";                                             
    return new TelegramBotClient(token);
});

builder.Services.AddScoped<ITgUserRepo, TgUserRepo>(); //////////////////////////////

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<MessageHandler>();

builder.Services.AddTransient<ICommand, StartCmd>();

builder.Services.AddSingleton<CommandProvider>();



builder.Services.AddHostedService<BotWorker>();



var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
