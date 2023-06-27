using Gtk;

class StripPanel : Button
{
    public StripPanel(MainWindow win) : base("开始精简")
    {
        Sensitive = false;
        Halign = Align.Center;
        Valign = Align.Center;
        HeightRequest = 64;
        WidthRequest = 224;

        win.GameSelector.GameChanged += gameConfig =>
        {
            Sensitive = gameConfig != null;
        };
    }
}