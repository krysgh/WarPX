using WarPX.Api.Hubs;
using WarPX.Application.Interfaces;
using WarPX.Application.Services;
using WarPX.Domain.Interfaces;
using WarPX.Infrastructure.Persistence;
using WarPX.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICanvasRepository, MemoryCanvasRepository>();
builder.Services.AddSingleton<ICooldownService>(_ => new CooldownService(cooldownSeconds: 0));
builder.Services.AddSingleton<IBotManager, BotManager>();
builder.Services.AddSingleton<ICanvasService, CanvasService>();
builder.Services.AddSingleton<IPixelBatchQueue, PixelBatchQueue>();
builder.Services.AddHostedService<PixelBatchBroadcaster>();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true)
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<PixelHub>("/pixelHub");

app.MapGet("/health", () => Results.Ok(new { status = "WarPX API Online", timestamp = DateTime.UtcNow }));

app.MapGet("/painel-restrito-warpx99", (HttpContext context, IWebHostEnvironment env) =>
{
    var key = context.Request.Query["key"].ToString();

    if (key != "warpx2026")
    {
        return Results.NotFound();
    }

    var filePath = Path.Combine(env.ContentRootPath, "Admin", "admin.html");
    return Results.File(filePath, "text/html");
});

app.Run();