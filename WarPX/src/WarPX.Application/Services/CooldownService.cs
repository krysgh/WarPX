using System.Collections.Concurrent;
using WarPX.Application.Interfaces;
using WarPX.Domain.ValueObjects;

namespace WarPX.Application.Services;

public class CooldownService : ICooldownService
{
    private readonly ConcurrentDictionary<string, DateTime> _lastPaintedTracker = new();
    private readonly CooldownRule _cooldownRule;

    public CooldownService(int cooldownSeconds = 1)
    {
        _cooldownRule = new CooldownRule(cooldownSeconds);
    }

    public bool CanPaint(string ipAddress, out double remainingSeconds)
    {
        _lastPaintedTracker.TryGetValue(ipAddress, out var lastPaintedAt);
        bool canPaint = _cooldownRule.CanPaint(lastPaintedAt == default ? null : lastPaintedAt, out var remainingTime);
        remainingSeconds = Math.Ceiling(remainingTime.TotalSeconds);
        return canPaint;
    }

    public void RegisterPaint(string ipAddress)
    {
        _lastPaintedTracker[ipAddress] = DateTime.UtcNow;
    }
}