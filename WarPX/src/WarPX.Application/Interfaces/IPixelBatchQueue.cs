using WarPX.Application.DTOs;

namespace WarPX.Application.Interfaces;

public interface IPixelBatchQueue
{
    void Enqueue(PixelUpdateDto pixel);
    List<PixelUpdateDto> DequeueAll();
}