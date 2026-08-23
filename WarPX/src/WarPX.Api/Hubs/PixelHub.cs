using Microsoft.AspNetCore.SignalR;
using WarPX.Application.DTOs;
using WarPX.Application.Interfaces;
using WarPX.Domain.Interfaces;

namespace WarPX.Api.Hubs;

public class PixelHub : Hub
{
    private readonly ICanvasService _canvasService;
    private readonly ICanvasRepository _repository;
    private readonly IPixelBatchQueue _batchQueue;
    private readonly IBotManager _botManager;

    public PixelHub(
        ICanvasService canvasService,
        ICanvasRepository repository,
        IPixelBatchQueue batchQueue,
        IBotManager botManager)
    {
        _canvasService = canvasService;
        _repository = repository;
        _batchQueue = batchQueue;
        _botManager = botManager;
    }

    public async Task<PaintResultDto> PaintPixel(int x, int y, string hexColor)
    {
        string userIp = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString() ?? Context.ConnectionId;

        var result = _canvasService.PaintPixel(x, y, hexColor, userIp);

        if (result.Success)
        {
            _batchQueue.Enqueue(new PixelUpdateDto(x, y, hexColor, userIp));
            Console.WriteLine($"[HUB] Enfileirado pixel ({x},{y})");
        }

        return result;
    }

    public CanvasStateDto GetCanvasState()
    {
        return _canvasService.GetCurrentState();
    }

    public async Task ClearRegion(int startX, int startY, int endX, int endY)
    {
        _canvasService.ClearRegion(startX, startY, endX, endY);
        await Clients.All.SendAsync("CanvasStateReset", _canvasService.GetCurrentState());
    }

    public void SaveSnapshot()
    {
        _repository.SaveSnapshot();
    }

    public async Task<bool> RestoreSnapshot()
    {
        bool restored = _repository.RestoreLastSnapshot();
        if (restored)
        {
            await Clients.All.SendAsync("CanvasStateReset", _canvasService.GetCurrentState());
        }
        return restored;
    }

    public async Task StartBots(int count, string[] colors, int delayMs)
    {
        _botManager.StartBots(count, colors, delayMs);
        await Clients.All.SendAsync("BotStatusChanged", _botManager.ActiveBotCount);
    }

    public async Task StopBots()
    {
        _botManager.StopBots();
        await Clients.All.SendAsync("BotStatusChanged", 0);
    }

    public int GetActiveBotCount()
    {
        return _botManager.ActiveBotCount;
    }
}