using Gdk;
using Gtk;


class MainWindow : Gtk.Window
{
    public readonly GameSelector GameSelector;

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
        GameSelector = new GameSelector(this);
        mainGrid.Attach(GameSelector, 0, 1, 1, 1);
        mainGrid.Attach(GameSelector.ButtonPanel, 0, 2, 1, 1);

        Notebook notebook = new()
        {
            Vexpand = true,
            Valign = Align.Fill
        };

        mainGrid.Attach(notebook, 0, 3, 1, 1);

        notebook.AppendPage(new ConvertPanel(this), new Label("转换"));
        notebook.AppendPage(new StripPanel(this), new Label("精简"));
        notebook.AppendPage(new PackPanel(this), new Label("打包"));
        notebook.AppendPage(new UnpackPanel(this), new Label("解包"));

        Add(mainGrid);
        ShowAll();
    }
}