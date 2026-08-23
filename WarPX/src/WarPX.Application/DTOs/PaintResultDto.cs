namespace WarPX.Application.DTOs;

public record PaintResultDto(bool Success, string? ErrorMessage, double RemainingCooldownSeconds);