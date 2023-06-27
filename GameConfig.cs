using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Versioning;

class GameConfig
{
    public string GameDir { get; }
    public string GameTitle { get; }
    public string GamePlatform { get; }
    public Tuple<int, int> ImageSize { get; }
    public string? IconPath
    {
        get
        {
            var path = Path.Combine(GameDir, "./icon.png");
            if (File.Exists(path)) return path;
            else return null;
        }
    }

    private readonly IReadOnlyDictionary<string, string[]> gameConfig;

    private GameConfig(string dir)
    {
        var gameConfigPath = Path.Combine(dir, "./gameconfig.txt")!;
        gameConfig = File.ReadAllLines(gameConfigPath)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Split(','))
            .ToDictionary(x => x[0], x => x[1..]);
        GameDir = dir;

        GameTitle = gameConfig["gametitle"][0].Replace("\\n", "\n");
        GamePlatform = gameConfig["platform"][0];

        var imageSize = gameConfig["imagesize"];
        ImageSize = Tuple.Create(
            int.Parse(imageSize[0]), int.Parse(imageSize[1]));
    }

    public static GameConfig? FromGameDir(string dir)
    {
        try
        {
            return new GameConfig(dir);
        }
        catch
        {
            return null;
        }
    }

    public void StartGame(Gtk.Window win)
    {
        if (CPyMOTools.CPyMOExecutable == null)
            Utils.Msgbox(win, "找不到cpymo可执行文件。");
        else
            System.Diagnostics.Process.Start(
                CPyMOTools.CPyMOExecutable!, GameDir);
    }

    [SupportedOSPlatform("windows")]
    public void CreateShortcutOnDesktop()
    {
        var desktopDir = Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop);
        var lnkFileName =
            GameTitle
                .Split('\n')
                .Where(x => !string.IsNullOrEmpty(x))
                .FirstOrDefault();

        var lnkPath = Path.Combine(desktopDir, lnkFileName + ".lnk");

        var shellType = Type.GetTypeFromProgID("WScript.Shell");
        dynamic shell = Activator.CreateInstance(shellType!)!;
        var shortcut = shell.CreateShortcut(lnkPath);
        shortcut.TargetPath = CPyMOTools.CPyMOExecutable;
        if (IconPath != null) shortcut.IconLocation = IconPath!;
        shortcut.WorkingDirectory = GameDir;
        shortcut.Save();
    }
}