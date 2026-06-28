using System.Security.Principal;

namespace XIFocus;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        if (!new WindowsPrincipal(WindowsIdentity.GetCurrent())
            .IsInRole(WindowsBuiltInRole.Administrator))
        {
            MessageBox.Show(
                "XI Focus requires administrator privileges to switch between FFXI windows.\n\nPlease run as administrator.",
                "XI Focus — Elevation Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}