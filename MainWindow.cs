using Gdk;
using Gtk;
using System;


class MainWindow : Gtk.Window
{
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

        mainGrid.Attach(new StatusPanel(), 0, 0, 1, 1);
        var gameSelector = new GameSelector(this);
        mainGrid.Attach(gameSelector, 0, 1, 1, 1);

        Grid centerButtons = new()
        {
            Halign = Align.Fill,
            Hexpand = true,
            ColumnSpacing = 4,
            ColumnHomogeneous = true
        };

        centerButtons.Attach(gameSelector.startGameButton, 0, 0, 1, 1);
        if (OperatingSystem.IsWindows())
        {
            centerButtons.Attach(gameSelector.createShortCutLink, 1, 0, 1, 1);
            centerButtons.Attach(gameSelector.openInFileExplorer, 2, 0, 1, 1);
        }

        mainGrid.Attach(centerButtons, 0, 2, 1, 1);

        Notebook notebook = new();
        mainGrid.Attach(notebook, 0, 3, 1, 1);

        notebook.AppendPage(new Grid(), new Label("转换"));
        notebook.AppendPage(new Grid(), new Label("精简"));
        notebook.AppendPage(new Grid(), new Label("打包"));
        notebook.AppendPage(new Grid(), new Label("解包"));

        Add(mainGrid);
        ShowAll();
    }
}