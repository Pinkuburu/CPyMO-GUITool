#pragma warning disable 0612

using Gtk;
using Gdk;

class StatusPanel : Grid
{
    public StatusPanel()
    {
        ColumnSpacing = 8;
        RowSpacing = 8;

        int currentRow = 0;
        void MakeInfoGrid(string title, string? path)
        {
            RGBA col = path == null ? Utils.ErrorColor : Utils.OKColor;

            Label titleLabel = new(title) { Halign = Align.End };
            titleLabel.OverrideColor(StateFlags.Normal, col);
            Attach(titleLabel, 0, currentRow, 1, 1);

            Label pathLabel = new(path ?? "（空）") { Halign = Align.Start };
            pathLabel.OverrideColor(StateFlags.Normal, col);
            Attach(pathLabel, 1, currentRow, 1, 1);
            currentRow++;
        }

        MakeInfoGrid("cpymo路径：", CPyMOTools.CPyMOExecutable);
        MakeInfoGrid("cpymo-tool路径：", CPyMOTools.CPyMOToolExecutable);
        MakeInfoGrid("ffmpeg路径：", CPyMOTools.FFMpegExecutable);
    }
}
