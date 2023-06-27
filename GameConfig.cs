using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

class GameConfig
{
    public string GameDir { get; }
    public string GameTitle { get; }
    public string GamePlatform { get; }
    public Tuple<int, int> ImageSize { get; }

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
}