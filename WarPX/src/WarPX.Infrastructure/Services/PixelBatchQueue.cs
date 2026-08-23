using System.Collections.Concurrent;
using WarPX.Application.DTOs;
using WarPX.Application.Interfaces;

namespace WarPX.Infrastructure.Services;

public class PixelBatchQueue : IPixelBatchQueue
{
    private readonly ConcurrentQueue<PixelUpdateDto> _queue = new();

    public void Enqueue(PixelUpdateDto pixel)
    {
        _queue.Enqueue(pixel);
    }

    public List<PixelUpdateDto> DequeueAll()
    {
        var items = new List<PixelUpdateDto>();
        while (_queue.TryDequeue(out var item))
        {
            items.Add(item);
        }
        return items;
    }
}