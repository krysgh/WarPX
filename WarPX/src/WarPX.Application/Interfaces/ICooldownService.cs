namespace WarPX.Application.Interfaces;

public interface ICooldownService
{
    bool CanPaint(string ipAddress, out double remainingSeconds);
    void RegisterPaint(string ipAddress);
}