using System;
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
        { Sensitive = gameConfig != null; };

        Clicked += OnClicked;
        mainWindow = win;
    }

    readonly MainWindow mainWindow;

    void OnClicked(object? sender, EventArgs e)
    {
        FileChooserNative fc = new(
            "输出到", mainWindow, FileChooserAction.CreateFolder,
            "精简", "取消")
        {
            Modal = true,
            SelectMultiple = false
        };

        if (fc.Run() == (int)ResponseType.Accept)
        {
            var outDir = fc.Filename;

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

            mainWindow.GameSelector.ClickToSelectGameDir.Sensitive = false;
            Sensitive = false;
            var gameDir = mainWindow.GameSelector.GameConfig!.GameDir;

            var prc = new System.Diagnostics.Process
            {
                StartInfo = new()
                {
                    FileName = CPyMOTools.CPyMOToolExecutable,
                    Arguments = $"strip \"{gameDir}\" \"{outDir}\" --pack"
                },

                EnableRaisingEvents = true
            };

            prc.Start();

            prc.Exited += (_0, _1) =>
            {
                mainWindow.GameSelector.ClickToSelectGameDir.Sensitive = true;
                Sensitive = true;

                if (prc.ExitCode == 0)
                {
                    Utils.Msgbox(mainWindow, "精简成功。");
                }
                else
                {
                    Utils.Msgbox(
                        mainWindow, "精简失败，错误代码：" + prc.ExitCode);
                }
            };
        }
    }
}
