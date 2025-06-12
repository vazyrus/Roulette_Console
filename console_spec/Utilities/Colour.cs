using Spectre.Console;

namespace console_spec.Utilities;

public static class Colour
{
    public static Color Red = new Color(239, 64, 64);
    public static Color Yellow = new Color(255, 247, 138);
    public static Color Blue = new Color(112, 184, 245);
    public static Color Cyan = new Color(139, 220, 247);
    public static Color LightPurple = new Color(172, 135, 197);

    public static Dictionary<RouletteColour, Color> Colours = new()
    {
        { RouletteColour.Red, Red },
        { RouletteColour.Yellow, Yellow },
        { RouletteColour.Blue, Blue },
        { RouletteColour.Cyan, Cyan },
        { RouletteColour.LightPurple, LightPurple },
    };
}