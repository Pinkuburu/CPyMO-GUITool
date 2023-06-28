using Gtk;

class PackPanel : Grid
{
    readonly MainWindow mainWindow;
    readonly Entry dirPathEntry = new() { Hexpand = true, Halign = Align.Fill };

    public PackPanel(MainWindow win)
    {
        mainWindow = win;
        Hexpand = true;
        Halign = Align.Fill;
        RowSpacing = 14;
        ColumnSpacing = 8;
        Margin = 24;

        Attach(new Label("要打包的文件夹：") { Halign = Align.End }, 0, 0, 1, 1);
        Attach(dirPathEntry, 1, 0, 1, 1);

        Button selectDir = new("选择") { Halign = Align.End };
        Attach(selectDir, 2, 0, 1, 1);

        Button start = new("打包")
        {
            Sensitive = false,
            HeightRequest = 64,
            WidthRequest = 224,
            Valign = Align.Center,
            Halign = Align.Center
        };

        Attach(start, 0, 1, 3, 1);

        dirPathEntry.Changed += (_0, _1) =>
        {
            start.Sensitive = System.IO.Directory.Exists(dirPathEntry.Text);
        };

        selectDir.Clicked += (_0, _1) =>
        {
            FileFilter filter = new();
            FileChooserNative fc = new(
                "打包...", win, FileChooserAction.SelectFolder,
                "选择", "取消")
            {
                Modal = true,
                SelectMultiple = false,
                Filter = filter
            };

            var res = fc.Run();
            fc.Destroy();

            if (res == (int)ResponseType.Accept)
                dirPathEntry.Text = fc.Filename;
        };
    }
}