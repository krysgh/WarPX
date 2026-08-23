using WarPX.Domain.ValueObjects;

namespace WarPX.Domain.Entities;

public class Canvas
{
    public int Width { get; }
    public int Height { get; }
    private readonly Pixel[,] _grid;

    public Canvas(int width = 100, int height = 100, string defaultColor = "#FFFFFF")
    {
        Width = width;
        Height = height;
        _grid = new Pixel[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _grid[x, y] = new Pixel(new Coordinate(x, y), defaultColor, "System");
            }
        }
    }

    public bool IsValidCoordinate(Coordinate coord) =>
        coord.X >= 0 && coord.X < Width && coord.Y >= 0 && coord.Y < Height;

    public Pixel? GetPixel(Coordinate coord)
    {
        if (!IsValidCoordinate(coord)) return null;
        return _grid[coord.X, coord.Y];
    }

    public bool SetPixel(Coordinate coord, string hexColor, string updatedByIp)
    {
        if (!IsValidCoordinate(coord)) return false;
        _grid[coord.X, coord.Y].UpdateColor(hexColor, updatedByIp);
        return true;
    }

    public void ClearRegion(Coordinate start, Coordinate end, string defaultColor = "#FFFFFF")
    {
        int startX = Math.Max(0, Math.Min(start.X, end.X));
        int endX = Math.Min(Width - 1, Math.Max(start.X, end.X));
        int startY = Math.Max(0, Math.Min(start.Y, end.Y));
        int endY = Math.Min(Height - 1, Math.Max(start.Y, end.Y));

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                _grid[x, y].UpdateColor(defaultColor, "Admin_Moderation");
            }
        }
    }

    public Pixel[,] GetGridState() => _grid;
}