using WarPX.Application.DTOs;

namespace WarPX.Application.Interfaces;

public interface ICanvasService
{
    CanvasStateDto GetCurrentState();
    PaintResultDto PaintPixel(int x, int y, string hexColor, string userIp);
    void ClearRegion(int startX, int startY, int endX, int endY);
}