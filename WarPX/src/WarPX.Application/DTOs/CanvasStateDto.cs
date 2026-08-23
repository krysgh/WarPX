namespace WarPX.Application.DTOs;

public class CanvasStateDto
{
    public int Width { get; set; }
    public int Height { get; set; }
    public List<PixelUpdateDto> Pixels { get; set; } = new();
}