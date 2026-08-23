using WarPX.Application.DTOs;
using WarPX.Application.Interfaces;

namespace WarPX.Infrastructure.Services;

public class BotManager : IBotManager
{
    private readonly ICanvasService _canvasService;
    private readonly IPixelBatchQueue _batchQueue;
    private CancellationTokenSource? _cts;
    private readonly object _lock = new();

    public int ActiveBotCount { get; private set; } = 0;

    public BotManager(ICanvasService canvasService, IPixelBatchQueue batchQueue)
    {
        _canvasService = canvasService;
        _batchQueue = batchQueue;
    }

    public void StartBots(int count, string[] colors, int delayMs = 100)
    {
        lock (_lock)
        {
            StopBots();

            if (count <= 0) return;

            if (colors == null || colors.Length == 0)
            {
                colors = new[] { "#FF0000", "#00FF00", "#0000FF", "#FFFF00", "#FF00FF", "#00FFFF" };
            }

            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            ActiveBotCount = count;

            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                int botId = i + 1;
                Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        int x = random.Next(0, 100);
                        int y = random.Next(0, 100);
                        string color = colors[random.Next(colors.Length)];

                        var result = _canvasService.PaintPixel(x, y, color, $"BOT_{botId}");
                        if (result.Success)
                        {
                            _batchQueue.Enqueue(new PixelUpdateDto(x, y, color, $"BOT_{botId}"));
                        }

                        try
                        {
                            await Task.Delay(delayMs, token);
                        }
                        catch (TaskCanceledException)
                        {
                            break;
                        }
                    }
                }, token);
            }
        }
    }

    public void StopBots()
    {
        lock (_lock)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            ActiveBotCount = 0;
        }
    }
}