using Gtk;

class GameSelector : Grid
{
    private readonly Label gameDirLabel = new("(空)");
    string? _currentSelectGameDir = null;
    public string? CurrentSelectedGameDir
    {
        get => _currentSelectGameDir;
        private set
        {
            gameDirLabel.OverrideColor(StateFlags.Normal,
                value == null ? Utils.ErrorColor : Utils.OKColor);
            gameDirLabel.Text = value ?? "（空）";
            _currentSelectGameDir = value;
        }
    }

    public GameSelector(Window window)
    {
        Halign = Align.Fill;
        ColumnSpacing = 16;

        Label label = new("游戏文件夹：") { Halign = Align.Start };
        Attach(label, 0, 0, 1, 1);
        Attach(gameDirLabel, 1, 0, 1, 1);
        Button clickToOpenGameDir = new("选择") { Halign = Align.End };
        Attach(clickToOpenGameDir, 2, 0, 1, 1);

        clickToOpenGameDir.Clicked += (_1, _2) =>
        {
            FileChooserNative gameSel = new(
                "选择游戏", window, FileChooserAction.SelectFolder, "选择", "取消")
            {
                Modal = true,
                SelectMultiple = false
            };

            if (gameSel.Run() == (int)ResponseType.Accept)
            {
                //TODO: Only valid gamedir will be passed
                // CurrentSelectedGameDir = gameSel.Filename;
            }
        };

    }
}