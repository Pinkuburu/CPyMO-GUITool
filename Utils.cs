using Gdk;

static class Utils
{
    public static RGBA OKColor =>
        new()
        {
            Blue = 63.0 / 255.0,
            Red = 7.0 / 255.0,
            Green = 105.0 / 255.0,
            Alpha = 1
        };

    public static RGBA ErrorColor =>
        new()
        {
            Alpha = 1,
            Red = 122.0 / 255.0,
            Green = 0,
            Blue = 2.0 / 255.0
        };
}