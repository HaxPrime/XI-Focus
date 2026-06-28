using System.Diagnostics;
using System.Reflection;

namespace XIFocus;

public partial class AboutForm : Form
{
    public AboutForm()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetName().Version?.ToString(3) ?? "1.0.0";

        string exePath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
        string hash = "(unavailable)";
        if (File.Exists(exePath))
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(exePath);
            var bytes = sha.ComputeHash(stream);
            hash = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        this.Text = "About XI Focus";
        this.ClientSize = new Size(380, 320);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Icon = IconGenerator.CreateAppIcon();

        // Steam theme
        this.BackColor = Color.FromArgb(27, 40, 56);
        this.ForeColor = Color.FromArgb(198, 212, 223);
        this.Font = new Font("Segoe UI", 9f);

        // Icon label
        var lblIcon = new Label
        {
            Text = "XI",
            Font = new Font("Segoe UI", 28f, FontStyle.Bold),
            ForeColor = Color.FromArgb(102, 192, 244),
            Location = new Point(20, 20),
            AutoSize = true
        };

        var lblName = new Label
        {
            Text = "XI Focus",
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            ForeColor = Color.FromArgb(198, 212, 223),
            Location = new Point(80, 22),
            AutoSize = true
        };

        var lblVersion = new Label
        {
            Text = $"Version {version}",
            ForeColor = Color.FromArgb(143, 152, 160),
            Location = new Point(80, 52),
            AutoSize = true
        };

        var lblDesc = new Label
        {
            Text = "FFXI Instance Switcher",
            ForeColor = Color.FromArgb(198, 212, 223),
            Location = new Point(20, 90),
            AutoSize = true
        };

        var lblStack = new Label
        {
            Text = "Built with C# / .NET 9  ·  SharpDX.XInput  ·  Win32 P/Invoke",
            ForeColor = Color.FromArgb(143, 152, 160),
            Font = new Font("Segoe UI", 8f),
            Location = new Point(20, 115),
            AutoSize = true
        };

        var lblAuthor = new Label
        {
            Text = "Author: HaxPrime",
            ForeColor = Color.FromArgb(143, 152, 160),
            Location = new Point(20, 145),
            AutoSize = true
        };

        var lblLicense = new Label
        {
            Text = "License: MIT",
            ForeColor = Color.FromArgb(143, 152, 160),
            Location = new Point(20, 165),
            AutoSize = true
        };

        var lblHash = new Label
        {
            Text = $"Build: {hash}",
            ForeColor = Color.FromArgb(143, 152, 160),
            Font = new Font("Segoe UI", 8f),
            Location = new Point(20, 185),
            AutoSize = true
        };
        lblHash.AutoSize = false;
        lblHash.Size = new Size(340, 40);
        lblHash.Font = new Font("Segoe UI", 7.5f);

        var btnGitHub = new Button
        {
            Text = "github.com/HaxPrime",
            Location = new Point(190, 265),
            Size = new Size(150, 28),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(76, 107, 34),
            ForeColor = Color.FromArgb(198, 212, 223),
            Cursor = Cursors.Hand
        };
        btnGitHub.FlatAppearance.BorderColor = Color.FromArgb(76, 107, 34);
        btnGitHub.FlatAppearance.MouseOverBackColor = Color.FromArgb(106, 150, 47);
        btnGitHub.Click += (s, e) =>
            Process.Start(new ProcessStartInfo("https://github.com/HaxPrime")
                { UseShellExecute = true });

        var btnCopyHash = new Button
        {
            Text = "Copy Build Info",
            Location = new Point(20, 265),
            Size = new Size(120, 28),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(49, 98, 130),
            ForeColor = Color.FromArgb(198, 212, 223),
            Cursor = Cursors.Hand
        };
        btnCopyHash.FlatAppearance.BorderColor = Color.FromArgb(27, 40, 56);
        btnCopyHash.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 130, 160);
        btnCopyHash.Click += (s, e) =>
        {
            Clipboard.SetText($"XI Focus v{version} | hash: {hash}");
            btnCopyHash.Text = "Copied!";
            Task.Delay(1500).ContinueWith(_ =>
                this.Invoke(() => btnCopyHash.Text = "Copy Build Info"));
        };

        this.Controls.AddRange(new Control[]
        {
            lblIcon, lblName, lblVersion, lblDesc, lblStack,
            lblAuthor, lblHash, lblLicense, btnGitHub, btnCopyHash
        });
    }
}
