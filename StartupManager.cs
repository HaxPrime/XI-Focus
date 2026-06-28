using System.Diagnostics;

namespace XIFocus;

internal static class StartupManager
{
    private const string TaskName = "XIFocus";

    public static bool IsEnabled()
    {
        try
        {
            var psi = new ProcessStartInfo("schtasks", $"/query /tn \"{TaskName}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using var p = Process.Start(psi);
            p?.WaitForExit();
            return p?.ExitCode == 0;
        }
        catch { return false; }
    }

    public static void Enable(string exePath)
    {
        try
        {
            // /rl HIGHEST = run elevated, /sc ONLOGON = at login, /f = force overwrite
            string args = $"/create /tn \"{TaskName}\" /tr \"\\\"{exePath}\\\"\" " +
                          $"/sc ONLOGON /rl HIGHEST /f";
            var psi = new ProcessStartInfo("schtasks", args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using var p = Process.Start(psi);
            p?.WaitForExit();
            if (p?.ExitCode != 0)
                throw new Exception($"schtasks exited with code {p?.ExitCode}");
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(
                $"Could not register startup task:\n{ex.Message}",
                "XI Focus", System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Warning);
        }
    }

    public static void Disable()
    {
        try
        {
            var psi = new ProcessStartInfo("schtasks", $"/delete /tn \"{TaskName}\" /f")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using var p = Process.Start(psi);
            p?.WaitForExit();
        }
        catch { }
    }

    public static string GetCurrentExePath() =>
        Process.GetCurrentProcess().MainModule?.FileName ?? "";
}
