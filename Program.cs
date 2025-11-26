using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TestShopApp;
using TestShopApp.App.Middlewares;
using TestShopApp.Common.Data;
using TestShopApp.Common.Repo;
using TestShopApp.Telegram.Commands;
using TestShopApp.Telegram.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

Env.Load();

builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var token = Environment.GetEnvironmentVariable("TOKEN");                                             
    return new TelegramBotClient(token);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IGroupRepo, GroupRepo>();

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
