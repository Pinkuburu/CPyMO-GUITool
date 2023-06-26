
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
        mainGrid.Attach(new GameSelector(this), 0, 1, 1, 1);

        Add(mainGrid);
        ShowAll();
    }
}