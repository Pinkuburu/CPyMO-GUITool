#pragma warning disable 0612, 0618

using Gtk;

class GameSelector : Grid
{
    readonly Label gameDirLabel = new() { Halign = Align.Start };
    readonly Image gameIcon = new() { Halign = Align.End, Valign = Align.Center };
    readonly Label gameTitleLabel = new() { Halign = Align.Start };
    readonly Label imageSizeLabel = new() { Halign = Align.End };
    readonly Label symbianLabel = new() { Halign = Align.End };
    public readonly Button startGameButton =
        new("开始游戏") { Halign = Align.Fill };

    GameConfig? _gameConfig;
    public GameConfig? GameConfig
    {
        get => _gameConfig;
        set
        {
            _gameConfig = value;
            startGameButton.Sensitive = value != null;
            gameIcon.Clear();

            if (value == null)
            {
                gameDirLabel.Text = "(空)";
                gameDirLabel.OverrideColor(StateFlags.Normal, Utils.ErrorColor);
                gameTitleLabel.Text = "";
                imageSizeLabel.Text = "";
                symbianLabel.Text = "";
            }
            else
            {
                gameDirLabel.Text = value.GameDir;
                gameDirLabel.OverrideColor(StateFlags.Normal, Utils.OKColor);
                gameTitleLabel.Text = value.GameTitle;
                imageSizeLabel.Text =
                    $"{value.ImageSize.Item1}x{value.ImageSize.Item2}";
                symbianLabel.Text = value.GamePlatform switch
                {
                    "s60v3" => "Symbian S60v3",
                    "s60v5" => "Symbian S60v5",
                    "pygame" => "",
                    (var _) => "(未知的游戏平台)"
                };

                if (value.IconPath != null)
                    gameIcon.FromFile = value.IconPath!;
            }
        }
    }

    public GameSelector(Window window)
    {
        Halign = Align.Fill;
        ColumnSpacing = 16;

        GameConfig = null;

        Attach(new Label("游戏文件夹：") { Halign = Align.Start }, 0, 0, 1, 1);
        Attach(gameDirLabel, 1, 0, 1, 1);
        Button clickToOpenGameDir = new("选择") { Halign = Align.End };
        Attach(clickToOpenGameDir, 2, 0, 1, 1);
        Attach(gameIcon, 0, 1, 1, 2);
        Attach(gameTitleLabel, 1, 1, 1, 2);
        Attach(imageSizeLabel, 2, 1, 1, 1);
        Attach(symbianLabel, 2, 2, 1, 1);

        clickToOpenGameDir.Clicked += (_1, _2) =>
        {
            FileChooserNative gameSel = new(
                "选择游戏", window, FileChooserAction.SelectFolder, "选择", "取消")
            {
                Modal = true,
                SelectMultiple = false
            };

            if (gameSel.Run() == (int)ResponseType.Accept)
            {
                var gameConfig = GameConfig.FromGameDir(gameSel.Filename);
                if (gameConfig == null)
                    Utils.Msgbox(
                        window,
                        $"\"{gameSel.Filename}\" " + "不是有效的pymo游戏。");
                else
                    GameConfig = gameConfig;
            }
        };

        startGameButton.Clicked += (_1, _2) => GameConfig!.StartGame(window);
    }
}