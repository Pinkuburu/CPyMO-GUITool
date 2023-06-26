using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

static class CPyMOTools
{
    public static string? CPyMOToolExecutable { get; private set; }
    public static string? CPyMOExecutable { get; private set; }
    public static string? FFMpegExecutable { get; private set; }

    static CPyMOTools()
    {
        char pathSep = ':';
        string exeSuffix = "";
        string pathEnv = "PATH";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            pathSep = ';';
            exeSuffix = ".exe";
        }

        var path =
            Environment
                .GetEnvironmentVariable(pathEnv)!
                .Split(pathSep)
                .ToList();

        path.Add(Environment.CurrentDirectory);

        CPyMOToolExecutable = FindExecutableFile(path, "cpymo-tool", exeSuffix);
        CPyMOExecutable = FindExecutableFile(path, "cpymo", exeSuffix);
        FFMpegExecutable = FindExecutableFile(path, "ffmpeg", exeSuffix);
    }

    static string? FindExecutableFile(
        IReadOnlyList<string> path,
        string name,
        string exeSuffix)
    {
        foreach (var dir in path)
        {
            var exePath = Path.Combine(dir, name + exeSuffix);
            if (File.Exists(exePath)) return exePath;
        }

        return null;
    }
}
