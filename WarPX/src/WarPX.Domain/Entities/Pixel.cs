using WarPX.Domain.ValueObjects;

namespace WarPX.Domain.Entities;

public class Pixel
{
    public Coordinate Coordinate { get; private set; }
    public string HexColor { get; private set; }
    public string UpdatedByIp { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    public Pixel(Coordinate coordinate, string hexColor, string updatedByIp)
    {
        Coordinate = coordinate;
        HexColor = hexColor;
        UpdatedByIp = updatedByIp;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateColor(string hexColor, string updatedByIp)
    {
        HexColor = hexColor;
        UpdatedByIp = updatedByIp;
        LastUpdatedAt = DateTime.UtcNow;
    }
}