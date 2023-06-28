using Gtk;
using System;

class UnpackPanel : Grid
{
    readonly Entry extNameEntry = new()
    {
        Valign = Align.Fill,
        Hexpand = true,
        Text = ".png"
    };

    readonly Entry pakFileEntry = new()
    {
        Valign = Align.Fill,
        Hexpand = true,
        Text = ""
    };

    readonly Button start = new("解包")
    {
        Sensitive = false,
        HeightRequest = 64,
        WidthRequest = 224,
        Valign = Align.Center,
        Halign = Align.Center
    };

    void UpdateStartButtonState()
    {
        bool s = true;
        if (!extNameEntry.Text.StartsWith("."))
            s = false;
        if (extNameEntry.Text.Length < 4)
            s = false;
        var pakPath = pakFileEntry.Text.Trim('\"', '\'');
        if (!pakPath.ToLower().EndsWith(".pak"))
            s = false;
        if (!System.IO.File.Exists(pakPath))
            s = false;
        start.Sensitive = s;
    }

    readonly MainWindow mainWindow;

    public UnpackPanel(MainWindow win)
    {
        mainWindow = win;
        Hexpand = true;
        Halign = Align.Fill;
        RowSpacing = 16;
        ColumnSpacing = 8;
        Margin = 24;

        extNameEntry.Changed += (_0, _1) => UpdateStartButtonState();
        pakFileEntry.Changed += (_0, _1) => UpdateStartButtonState();

        Button selectDirButton = new("选择");

        Attach(new Label("包：") { Halign = Align.End }, 0, 0, 1, 1);
        Attach(pakFileEntry, 1, 0, 1, 1);
        Attach(selectDirButton, 2, 0, 1, 1);
        Attach(new Label("扩展名：") { Halign = Align.End }, 0, 2, 1, 1);
        Attach(extNameEntry, 1, 2, 1, 1);

        start.Clicked += StartClicked;
        Attach(start, 0, 3, 3, 1);

        selectDirButton.Clicked += (_0, _1) =>
        {
            FileFilter fileFilter = new()
            {
                Name = "PyMO Package"
            };
            fileFilter.AddPattern("*.pak");

            FileChooserNative pakSel = new(
                "选择包文件", win, FileChooserAction.Open,
                "选择", "取消")
            {
                Modal = true,
                SelectMultiple = false,
                Filter = fileFilter
            };

            if (win.GameSelector.GameConfig != null)
                pakSel.SetCurrentFolder(win.GameSelector.GameConfig.GameDir);

            if (pakSel.Run() == (int)ResponseType.Accept)
            {
                var pak = pakSel.Filename;
                if (!pak.ToLower().EndsWith(".pak"))
                {
                    Utils.Msgbox(win, $"\"{pak}\" 不是pak文件。");
                    return;
                }

                pakFileEntry.Text = pak;
            }

            pakSel.Destroy();
        };

        pakFileEntry.Changed += (_, _) =>
        {
            var pakPath = pakFileEntry.Text.Trim('\'', '\"');
            var pakName = System.IO.Path.GetFileNameWithoutExtension(pakPath);
            var assetType = pakName.ToLower();

            string formatKey;
            switch (assetType)
            {
                case "bg": formatKey = "bgformat"; break;
                case "chara": formatKey = "charaformat"; break;
                case "se": formatKey = "seformat"; break;
                case "voice": formatKey = "voiceformat"; break;
                default: return;
            }

            var gameConfigPath =
                System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(pakPath)!,
                    "../");

            var gameConfig = GameConfig.FromGameDir(gameConfigPath);
            if (gameConfig != null)
            {
                var ext = gameConfig.GetValue(formatKey);
                if (ext != null) extNameEntry.Text = ext;
            }
        };
    }

    void StartClicked(object? sender, EventArgs e)
    {
        FileChooserNative fileChooser = new(
            "输出到文件夹", mainWindow, FileChooserAction.SelectFolder,
            "解包", "取消")
        {
            Modal = true,
            SelectMultiple = false
        };

        if (fileChooser.Run() == (int)ResponseType.Accept)
            Unpack(pakFileEntry.Text, extNameEntry.Text, fileChooser.Filename);
        fileChooser.Destroy();
    }

    void Unpack(string pakPath, string ext, string outDir)
    {
        pakPath = pakPath.Trim('\"', '\'');
        if (!System.IO.File.Exists(pakPath))
        {
            Utils.Msgbox(mainWindow, $"\"{pakPath}\" 不存在。");
            return;
        }

        if (!System.IO.Directory.Exists(outDir))
        {
            try { System.IO.Directory.CreateDirectory(outDir); }
            catch
            {
                Utils.Msgbox(mainWindow, $"不能创建文件夹 \"{outDir}\"");
                return;
            }
        }

        if (CPyMOTools.CPyMOToolExecutable == null)
        {
            Utils.Msgbox(mainWindow, "未能找到cpymo-tool。");
            return;
        }

        Sensitive = false;
        var prc = new System.Diagnostics.Process
        {
            StartInfo = new()
            {
                FileName = CPyMOTools.CPyMOToolExecutable,
                Arguments = $"unpack \"{pakPath}\" {ext} \"{outDir}\""
            },

            EnableRaisingEvents = true
        };

        prc.Start();

        prc.Exited += (_0, _1) =>
        {
            Sensitive = true;
            if (prc.ExitCode == 0)
                Utils.Msgbox(mainWindow, "解包成功。");
            else
                Utils.Msgbox(mainWindow, "解包失败，错误代码：" + prc.ExitCode);
        };
    }
}