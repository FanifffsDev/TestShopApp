using Telegram.Bot;
using TestShopApp;
using TestShopApp.App.Middlewares;
using TestShopApp.Common.Data;
using TestShopApp.Common.Repo;
using TestShopApp.Telegram.Commands;
using TestShopApp.Telegram.Handlers;

using Microsoft.EntityFrameworkCore;
using TestShopApp.App;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var token = "8344409652:AAHcBXyAMfqsZxZ3M5tSqlIKjKw6IEfLJ8g";                                             
    return new TelegramBotClient(token);
});

builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITelegramDataProcessor, TelegramDataProcessor>();

builder.Services.AddScoped<ITgUserRepo, TgUserRepo>(); //////////////////////////////

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<MessageHandler>();

builder.Services.AddTransient<ICommand, StartCmd>();

builder.Services.AddSingleton<CommandProvider>();

builder.Services.AddHostedService<BotWorker>();



var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.MapGet("/", async context =>
{
    context.Response.Redirect("/home");
});


app.Run();
