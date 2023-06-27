#pragma warning disable 0612, 0618

using System;
using Gtk;

class GameSelector : Grid
{
    readonly Label gameDirLabel = new() { Halign = Align.Start };
    readonly Image gameIcon = new() { Halign = Align.End, Valign = Align.Center };
    readonly Label gameTitleLabel = new() { Halign = Align.Start };
    readonly Label imageSizeLabel = new() { Halign = Align.End };
    readonly Label symbianLabel = new() { Halign = Align.End };
    readonly Button startGameButton =
        new("开始游戏") { Halign = Align.Fill, Hexpand = true };

    public event Action<GameConfig?>? GameChanged;

    readonly Button createShortCutLink =
        new("创建桌面快捷方式") { Halign = Align.Fill, Hexpand = true };

    readonly Button openInFileExplorer =
        new("打开文件夹") { Halign = Align.Fill, Hexpand = true };

    public Grid ButtonPanel
    {
        get
        {
            Grid buttonPanel = new()
            {
                Halign = Align.Fill,
                Hexpand = true,
                ColumnSpacing = 4,
                ColumnHomogeneous = true
            };

            buttonPanel.Attach(startGameButton, 0, 0, 1, 1);
            if (OperatingSystem.IsWindows())
            {
                buttonPanel.Attach(createShortCutLink, 1, 0, 1, 1);
                buttonPanel.Attach(openInFileExplorer, 2, 0, 1, 1);
            }

            return buttonPanel;
        }
    }

    GameConfig? _gameConfig;
    public GameConfig? GameConfig
    {
        get => _gameConfig;
        set
        {
            _gameConfig = value;
            startGameButton.Sensitive = value != null;
            createShortCutLink.Sensitive = startGameButton.Sensitive;
            openInFileExplorer.Sensitive = startGameButton.Sensitive;
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
                symbianLabel.Text = value.GamePlatform.ToLower() switch
                {
                    "s60v3" => "Symbian S60v3",
                    "s60v5" => "Symbian S60v5",
                    "pygame" => "",
                    var _ => "(未知的游戏平台)"
                };

                if (value.IconPath != null)
                    gameIcon.FromFile = value.IconPath!;
            }

            GameChanged?.Invoke(_gameConfig);
        }
    }

    public GameSelector(Window window)
    {
        Halign = Align.Fill;
        ColumnSpacing = 16;

        GameConfig = null;

        Attach(new Label("游戏文件夹：") { Halign = Align.Start }, 0, 0, 1, 1);
        Attach(gameDirLabel, 1, 0, 1, 1);
        Button clickToSelectGameDir = new("选择") { Halign = Align.End };
        Attach(clickToSelectGameDir, 2, 0, 1, 1);
        Attach(gameIcon, 0, 1, 1, 2);
        Attach(gameTitleLabel, 1, 1, 1, 2);
        Attach(imageSizeLabel, 2, 1, 1, 1);
        Attach(symbianLabel, 2, 2, 1, 1);

        clickToSelectGameDir.Clicked += (_1, _2) =>
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

        startGameButton.Clicked += (_1, _2) =>
        {
            clickToSelectGameDir.Sensitive = false;
            startGameButton.Sensitive = false;
            GameConfig!.StartGame(window, _ =>
            {
                clickToSelectGameDir.Sensitive = true;
                startGameButton.Sensitive = true;
            });
        };

        createShortCutLink.Clicked += (_1, _2) =>
        {
            if (OperatingSystem.IsWindows())
                GameConfig!.CreateShortcutOnDesktop();
            Utils.Msgbox(window, "已成功创建快捷方式！");
        };

        openInFileExplorer.Clicked += (_1, _2) =>
        {
            if (OperatingSystem.IsWindows())
                GameConfig!.OpenInFileExplorer();
        };
    }
}