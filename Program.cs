using System;
using Gtk;
class Program
{
    [STAThread]
    public static void Main()
    {
        Application.Init();

        var app = new Application("org.CPyMO_GUITool.CPyMO_GUITool", GLib.ApplicationFlags.None);
        app.Register(GLib.Cancellable.Current);

        var win = new MainWindow();
        app.AddWindow(win);

        win.Show();
        Application.Run();
    }
}
