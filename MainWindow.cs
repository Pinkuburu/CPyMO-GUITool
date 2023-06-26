#pragma warning disable 0612

using Gdk;
using Gtk;
using System;

class MainWindow : Gtk.Window
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

    private static Grid MakeInfoPanel()
    {
        Grid grid = new()
        {
            ColumnSpacing = 8,
            RowSpacing = 8,
        };

        int currentRow = 0;
        void MakeInfoGrid(string title, string? path)
        {
            RGBA col = path == null ? ErrorColor : OKColor;

            Label titleLabel = new(title) { Halign = Align.End };
            titleLabel.OverrideColor(StateFlags.Normal, col);
            grid.Attach(titleLabel, 0, currentRow, 1, 1);

            Label pathLabel = new(path ?? "（空）") { Halign = Align.Start };
            pathLabel.OverrideColor(StateFlags.Normal, col);
            grid.Attach(pathLabel, 1, currentRow, 1, 1);
            currentRow++;
        }

        MakeInfoGrid("cpymo路径：", CPyMOTools.CPyMOExecutable);
        MakeInfoGrid("cpymo-tool路径：", CPyMOTools.CPyMOToolExecutable);
        MakeInfoGrid("ffmpeg路径：", CPyMOTools.FFMpegExecutable);

        return grid;
    }

    #region 游戏选择器
    private readonly Label gameDirLabel = new("(空)");
    string? _currentSelectGameDir = null;
    public string? CurrentSelectedGameDir
    {
        get => _currentSelectGameDir;
        private set
        {
            gameDirLabel.OverrideColor(StateFlags.Normal,
                value == null ? ErrorColor : OKColor);
            gameDirLabel.Text = value ?? "（空）";
            _currentSelectGameDir = value;
        }
    }

    Grid MakeGameDirSelector()
    {
        Grid grid = new()
        {
            Halign = Align.Fill,
            ColumnSpacing = 16,
        };

        Label label = new("游戏文件夹：") { Halign = Align.Start };
        grid.Attach(label, 0, 0, 1, 1);
        grid.Attach(gameDirLabel, 1, 0, 1, 1);
        Button clickToOpenGameDir = new("选择") { Halign = Align.End };
        grid.Attach(clickToOpenGameDir, 2, 0, 1, 1);

        clickToOpenGameDir.Clicked += (_1, _2) =>
        {
            FileChooserNative gameSel = new(
                "选择游戏", this, FileChooserAction.SelectFolder, "选择", "取消")
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

        return grid;
    }
    #endregion

    public MainWindow() : base("CPyMO GUI Tool")
    {
        SetPosition(WindowPosition.Center);
        TypeHint = WindowTypeHint.Menu;

        DeleteEvent += delegate { Application.Quit(); };

        Grid mainGrid = new()
        {
            RowSpacing = 16,
            Margin = 32
        };

        mainGrid.Attach(MakeInfoPanel(), 0, 0, 1, 1);
        mainGrid.Attach(MakeGameDirSelector(), 0, 1, 1, 1);

        Add(mainGrid);
        ShowAll();
    }
}