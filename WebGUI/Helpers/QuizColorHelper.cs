namespace WebGUI.Helpers;

public static class QuizColorHelper
{
    private const string DefaultBackground = "#5BB933";
    private const string LightText = "#FFFFFF";
    private const string DarkText = "#111111";
    private const string LightShadow = "0 2px 12px rgba(255, 255, 255, 0.28)";
    private const string DarkShadow = "0 2px 12px rgba(0, 0, 0, 0.35)";

    public static string GetReadableTextColor(string? backgroundColor)
    {
        var (red, green, blue) = ParseColor(backgroundColor);
        var luminance = GetRelativeLuminance(red, green, blue);

        return luminance > 0.5 ? DarkText : LightText;
    }

    public static string GetReadableTextShadow(string? backgroundColor)
    {
        return GetReadableTextColor(backgroundColor) == DarkText
            ? LightShadow
            : DarkShadow;
    }

    private static (int Red, int Green, int Blue) ParseColor(string? backgroundColor)
    {
        var color = string.IsNullOrWhiteSpace(backgroundColor)
            ? DefaultBackground
            : backgroundColor.Trim();

        if (color.StartsWith('#'))
        {
            color = color[1..];
        }

        if (color.Length == 3)
        {
            color = string.Concat(color.Select(channel => $"{channel}{channel}"));
        }

        if (color.Length != 6
            || !int.TryParse(color[..2], System.Globalization.NumberStyles.HexNumber, null, out var red)
            || !int.TryParse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var green)
            || !int.TryParse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var blue))
        {
            return ParseColor(DefaultBackground);
        }

        return (red, green, blue);
    }

    private static double GetRelativeLuminance(int red, int green, int blue)
    {
        var normalizedRed = TransformChannel(red / 255d);
        var normalizedGreen = TransformChannel(green / 255d);
        var normalizedBlue = TransformChannel(blue / 255d);

        return (0.2126 * normalizedRed) + (0.7152 * normalizedGreen) + (0.0722 * normalizedBlue);
    }

    private static double TransformChannel(double channel)
    {
        return channel <= 0.03928
            ? channel / 12.92
            : Math.Pow((channel + 0.055) / 1.055, 2.4);
    }
}
