namespace WarPX.Application.Interfaces;

public interface IBotManager
{
    void StartBots(int count, string[] colors, int delayMs);
    void StopBots();
    int ActiveBotCount { get; }
}