using WarPX.Application.DTOs;
using WarPX.Application.Interfaces;
using WarPX.Domain.Interfaces;
using WarPX.Domain.ValueObjects;

namespace WarPX.Application.Services;

public class CanvasService : ICanvasService
{
    private readonly ICanvasRepository _repository;
    private readonly ICooldownService _cooldownService;

    public CanvasService(ICanvasRepository repository, ICooldownService cooldownService)
    {
        _repository = repository;
        _cooldownService = cooldownService;
    }

    public CanvasStateDto GetCurrentState()
    {
        var canvas = _repository.GetCanvas();
        var grid = canvas.GetGridState();
        var pixels = new List<PixelUpdateDto>();

        for (int x = 0; x < canvas.Width; x++)
        {
            for (int y = 0; y < canvas.Height; y++)
            {
                var pixel = grid[x, y];
                pixels.Add(new PixelUpdateDto(pixel.Coordinate.X, pixel.Coordinate.Y, pixel.HexColor, pixel.UpdatedByIp));
            }
        }

        return new CanvasStateDto
        {
            Width = canvas.Width,
            Height = canvas.Height,
            Pixels = pixels
        };
    }

    public PaintResultDto PaintPixel(int x, int y, string hexColor, string userIp)
    {
        if (!_cooldownService.CanPaint(userIp, out var remainingSeconds))
        {
            return new PaintResultDto(false, $"Aguarde {remainingSeconds}s para pintar novamente.", remainingSeconds);
        }

        var canvas = _repository.GetCanvas();
        var coord = new Coordinate(x, y);

        if (!canvas.IsValidCoordinate(coord))
        {
            return new PaintResultDto(false, "Coordenada fora dos limites do Canvas.", 0);
        }

        bool updated = canvas.SetPixel(coord, hexColor, userIp);
        if (updated)
        {
            _cooldownService.RegisterPaint(userIp);
            return new PaintResultDto(true, null, 10);
        }

        return new PaintResultDto(false, "Falha ao atualizar o pixel.", 0);
    }

    public void ClearRegion(int startX, int startY, int endX, int endY)
    {
        var canvas = _repository.GetCanvas();
        canvas.ClearRegion(new Coordinate(startX, startY), new Coordinate(endX, endY));
    }
}