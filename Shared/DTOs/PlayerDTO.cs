namespace WebGUI.DTOs;

public record PlayerDTO(string? PlayerId, string Name, uint Points, string? AvaterPic = null)
{};