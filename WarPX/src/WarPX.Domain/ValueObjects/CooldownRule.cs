namespace WarPX.Domain.ValueObjects;

public class CooldownRule
{
    public TimeSpan CooldownTime { get; }

    public CooldownRule(int seconds = 10)
    {
        CooldownTime = TimeSpan.FromSeconds(seconds);
    }

    public bool CanPaint(DateTime? lastPaintedAt, out TimeSpan remainingTime)
    {
        if (!lastPaintedAt.HasValue)
        {
            remainingTime = TimeSpan.Zero;
            return true;
        }

        var elapsed = DateTime.UtcNow - lastPaintedAt.Value;
        if (elapsed >= CooldownTime)
        {
            remainingTime = TimeSpan.Zero;
            return true;
        }

        remainingTime = CooldownTime - elapsed;
        return false;
    }
}