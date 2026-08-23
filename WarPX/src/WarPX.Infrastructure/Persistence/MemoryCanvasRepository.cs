using WarPX.Domain.Entities;
using WarPX.Domain.Interfaces;
using WarPX.Domain.ValueObjects;

namespace WarPX.Infrastructure.Persistence;

public class MemoryCanvasRepository : ICanvasRepository
{
    private readonly Canvas _canvas;
    private readonly object _lock = new();
    private Pixel[,]? _lastSnapshot;

    public MemoryCanvasRepository(int width = 100, int height = 100)
    {
        _canvas = new Canvas(width, height);
    }

    public Canvas GetCanvas()
    {
        return _canvas;
    }

    public void SaveSnapshot()
    {
        lock (_lock)
        {
            int width = _canvas.Width;
            int height = _canvas.Height;
            _lastSnapshot = new Pixel[width, height];
            var currentGrid = _canvas.GetGridState();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var original = currentGrid[x, y];
                    _lastSnapshot[x, y] = new Pixel(
                        new Coordinate(original.Coordinate.X, original.Coordinate.Y),
                        original.HexColor,
                        original.UpdatedByIp
                    );
                }
            }
        }
    }

    public bool RestoreLastSnapshot()
    {
        lock (_lock)
        {
            if (_lastSnapshot == null) return false;

            int width = _canvas.Width;
            int height = _canvas.Height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var saved = _lastSnapshot[x, y];
                    _canvas.SetPixel(saved.Coordinate, saved.HexColor, saved.UpdatedByIp);
                }
            }

            return true;
        }
    }
}