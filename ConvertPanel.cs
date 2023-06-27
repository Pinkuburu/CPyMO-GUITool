using Gtk;
using System;
using System.Linq;

class ConvertPanel : Grid
{
    readonly ComboBox targetSpecSelector;

    struct SpecInfo
    {
        public string Name;
        public int TargetWidth;
        public int TargetHeight;
        public string Description;

        public SpecInfo(string name, int w, int h, string desc)
        {
            Name = name;
            TargetWidth = w;
            TargetHeight = h;
            Description = desc;
        }
    };

    readonly SpecInfo[] specs = new SpecInfo[]
    {
        new("s60v3", 320, 240, "适用于塞班S60v3上的PyMO"),
        new("s60v5", 540, 360, "适用于塞班S60v5上的PyMO"),
        new("pymo", 800, 600, "适用于Windows/Android版本的PyMO"),
        new("3ds", 400, 300, "适用于任天堂3DS系列上运行的CPyMO"),
        new("psp", 480, 272, "适用于索尼PSP系列上运行的CPyMO"),
        new("wii", 640, 480, "适用于任天堂Wii上运行的CPyMO")
    };

    public ConvertPanel(MainWindow win)
    {
        Hexpand = true;
        Halign = Align.Fill;
        RowSpacing = 16;
        ColumnSpacing = 8;
        Margin = 24;

        Label targetSpecDescLabel = new()
        {
            Hexpand = true,
            Halign = Align.Start
        };

        targetSpecSelector = new(specs.Select(x => x.Name).ToArray())
        {
            Hexpand = true,
            Halign = Align.Fill,
        };

        targetSpecSelector.Changed += (_1, _2) =>
        {
            var spec = specs[targetSpecSelector.Active];
            var desc =
                $"分辨率：{spec.TargetWidth}x{spec.TargetHeight}"
                + Environment.NewLine
                + spec.Description;
            targetSpecDescLabel.Text = desc;
        };

        targetSpecSelector.Active = 0;

        Attach(new Label("目标平台：") { Halign = Align.End }, 0, 0, 1, 1);
        Attach(targetSpecSelector, 1, 0, 1, 1);
        Attach(targetSpecDescLabel, 1, 1, 1, 1);

        Button start = new("开始转换")
        {
            Halign = Align.Center,
            HeightRequest = 64,
            WidthRequest = 224,
            Sensitive = false
        };

        Attach(start, 0, 2, 2, 1);

        win.GameSelector.GameChanged += gameConfig =>
        {
            start.Sensitive = gameConfig != null;
        };
    }
}
