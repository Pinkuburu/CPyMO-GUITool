using System;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Drawing.Imaging;

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

    #if NET
    [SupportedOSPlatform("windows")]
    #endif
    public void OpenInFileExplorer()
    {
        System.Diagnostics.Process.Start("explorer.exe", GameDir);
    }

    #if NET
    [SupportedOSPlatform("windows")]
    #endif
    string? EnsureWindowsIcoFile()
    {
        if (IconPath == null) return null;

        var icoPath = Path.Combine(GameDir, "icon.ico");
        if (File.Exists(icoPath)) return icoPath;
        if (!BitConverter.IsLittleEndian) return null;

        using Bitmap iconBitmapOrg = new(IconPath!);
        using Bitmap iconBitmap = new(iconBitmapOrg, 64, 64);
        using MemoryStream iconBitmapStream = new();
        iconBitmap.Save(iconBitmapStream, ImageFormat.Png);
        iconBitmapStream.Flush();
        using var fs = File.Open(icoPath, FileMode.Create);
        using var icoWriter = new BinaryWriter(fs);

        icoWriter.Write((byte)0);
        icoWriter.Write((byte)0);

        icoWriter.Write((short)1);
        icoWriter.Write((short)1);
        icoWriter.Write((byte)iconBitmap.Width);
        icoWriter.Write((byte)iconBitmap.Height);

        icoWriter.Write((byte)0);
        icoWriter.Write((byte)0);
        icoWriter.Write((short)0);
        icoWriter.Write((short)32);
        icoWriter.Write((int)iconBitmapStream.Length);
        icoWriter.Write(6 + 16);
        icoWriter.Write(iconBitmapStream.ToArray());
        icoWriter.Flush();

        icoWriter.Close();
        fs.Close();

        return icoPath;
    }

    private readonly IReadOnlyDictionary<string, string[]> gameConfig;

    public string? GetValue(string key)
    {
        try { return gameConfig[key][0]; }
        catch { return null; }
    }

    private GameConfig(string dir)
    {
        var gameConfigPath = Path.Combine(dir, "./gameconfig.txt")!;
        gameConfig = File.ReadAllLines(gameConfigPath)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Split(','))
            .ToDictionary(x => x[0], x => x.Skip(1).ToArray());
        GameDir = dir;

        try { GameTitle = gameConfig["gametitle"][0].Replace("\\n", "\n"); }
        catch { GameTitle = "Unknown"; }

        try { GamePlatform = gameConfig["platform"][0]; }
        catch { GamePlatform = ""; }

        var imageSize = gameConfig["imagesize"];
        ImageSize = Tuple.Create(
            int.Parse(imageSize[0]), int.Parse(imageSize[1]));
    }

    public static GameConfig? FromGameDir(string dir)
    {
        try { return new GameConfig(dir); }
        catch { return null; }
    }

    public void StartGame(Gtk.Window win, Action<int>? onExited = null)
    {
        if (CPyMOTools.CPyMOExecutable == null)
        {
            Utils.Msgbox(win, "找不到cpymo可执行文件。");
            if (onExited != null) onExited!(-1);
        }
        else
        {
            System.Diagnostics.ProcessStartInfo startInfo = new()
            {
                WorkingDirectory = GameDir,
                FileName = CPyMOTools.CPyMOExecutable
            };

            var prc = new System.Diagnostics.Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            prc.Start();

            prc.Exited += (x, y) =>
            {
                if (onExited != null)
                    onExited!(prc.ExitCode);
            };
        }
    }

    #if NET
    [SupportedOSPlatform("windows")]
    #endif
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
        if (IconPath != null) shortcut.IconLocation = EnsureWindowsIcoFile();
        shortcut.WorkingDirectory = GameDir;
        shortcut.Save();
    }
}