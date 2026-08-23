using WarPX.Domain.Entities;

namespace WarPX.Domain.Interfaces;

public interface ICanvasRepository
{
    Canvas GetCanvas();
    void SaveSnapshot();
    bool RestoreLastSnapshot();
}