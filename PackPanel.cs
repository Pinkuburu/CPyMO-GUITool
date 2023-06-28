using Gtk;
using System;
using System.IO;

class PackPanel : Grid
{
    readonly Entry dirPathEntry = new() { Hexpand = true, Halign = Align.Fill };
    private readonly MainWindow mainWindow;

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
            FileChooserNative fc = new(
                "打包...", win, FileChooserAction.SelectFolder,
                "选择", "取消")
            {
                Modal = true,
                SelectMultiple = false
            };

            var res = fc.Run();
            fc.Destroy();

            if (res == (int)ResponseType.Accept)
                dirPathEntry.Text = fc.Filename;
        };

        start.Clicked += (_0, _1) =>
        {
            FileFilter filter = new() { Name = "PyMO Package" };
            filter.AddPattern("*.pak");

            FileChooserNative fc = new(
                "打包到", win, FileChooserAction.Save,
                "保存", "取消")
            {
                Modal = true,
                SelectMultiple = false
            };

            var res = fc.Run();
            fc.Destroy();

            if (res == (int)ResponseType.Accept)
            {
                var pakFile = fc.Filename;
                var dirToPack = dirPathEntry.Text;
                var filesToPack = Directory.GetFiles(
                    dirToPack, "*", SearchOption.TopDirectoryOnly);

                PackFile(pakFile, filesToPack);
            }
        };
    }

    private void PackFile(string pakFile, string[] dirToPack)
    {
        var listFile = System.IO.Path.GetTempFileName();
        File.WriteAllLines(listFile, dirToPack);

        var args = $"pack \"{pakFile}\" --file-list \"{listFile}\"";

        Sensitive = false;
        System.Diagnostics.Process prc = new()
        {
            StartInfo = new()
            {
                FileName = CPyMOTools.CPyMOToolExecutable,
                Arguments = args
            },

            EnableRaisingEvents = true
        };

        prc.Start();

        prc.Exited += (_0, _1) =>
        {
            Sensitive = true;
            if (prc.ExitCode == 0)
                Utils.Msgbox(mainWindow, "打包成功。");
            else
                Utils.Msgbox(mainWindow, $"打包失败，错误代码：{prc.ExitCode}");
        };
    }
}