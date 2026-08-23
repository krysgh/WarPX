using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using WarPX.Application.Interfaces;

namespace WarPX.Infrastructure.Services;

public class PixelBatchBroadcaster : BackgroundService
{
    private readonly IPixelBatchQueue _queue;
    private readonly IHubContext<Hub> _hubContext;

    public PixelBatchBroadcaster(IPixelBatchQueue queue, IHubContext<WarPX.Api.Hubs.PixelHub> hubContext)
    {
        _queue = queue;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var batch = _queue.DequeueAll();
            if (batch.Count > 0)
            {
                Console.WriteLine($"[BROADCASTER] Disparando lote com {batch.Count} pixels");
                await _hubContext.Clients.All.SendAsync("PixelsBatchUpdated", batch, stoppingToken);
            }
        }
    }
}