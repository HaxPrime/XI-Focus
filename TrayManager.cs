namespace XIFocus;

internal class TrayManager : IDisposable
{
    private readonly NotifyIcon _trayIcon;
    private readonly ContextMenuStrip _trayMenu;
    private readonly MainForm _mainForm;

    public TrayManager(MainForm mainForm)
    {
        _mainForm = mainForm;

        _trayMenu = new ContextMenuStrip();

        var showItem = new ToolStripMenuItem("Show XI Focus");
        showItem.Click += (s, e) =>
        {
            _mainForm.Show();
            _mainForm.BringToFront();
        };
        _trayMenu.Items.Add(showItem);

        _trayMenu.Items.Add(new ToolStripSeparator());

        var refreshItem = new ToolStripMenuItem("Refresh Windows");
        refreshItem.Click += (s, e) => _mainForm.RefreshWindows();
        _trayMenu.Items.Add(refreshItem);

        var optionsItem = new ToolStripMenuItem("Options");
        optionsItem.Click += (s, e) =>
        {
            if (new OptionsForm(_mainForm.GetSettings()).ShowDialog(_mainForm) == DialogResult.OK)
            {
                _mainForm.RebindManagers();
            }
        };
        _trayMenu.Items.Add(optionsItem);

        var miAbout = new ToolStripMenuItem("About XI Focus");
        miAbout.Click += (s, e) => new AboutForm().ShowDialog(_mainForm);
        miAbout.BackColor = Color.FromArgb(27, 40, 56);
        miAbout.ForeColor = Color.FromArgb(198, 212, 223);
        _trayMenu.Items.Add(miAbout);

        _trayMenu.Items.Add(new ToolStripSeparator());

        var exitItem = new ToolStripMenuItem("Exit XI Focus");
        exitItem.Click += (s, e) => Application.Exit();
        _trayMenu.Items.Add(exitItem);

        _trayMenu.BackColor = Color.FromArgb(27, 40, 56);
        _trayMenu.ForeColor = Color.FromArgb(198, 212, 223);
        _trayMenu.Font = new Font("Segoe UI", 9f);

        foreach (ToolStripItem item in _trayMenu.Items)
        {
            item.BackColor = Color.FromArgb(27, 40, 56);
            item.ForeColor = Color.FromArgb(198, 212, 223);
        }

        _trayMenu.RenderMode = ToolStripRenderMode.System;

        _trayIcon = new NotifyIcon
        {
            Text = "XI Focus",
            ContextMenuStrip = _trayMenu,
            Visible = true,
            Icon = SystemIcons.Application
        };
        _trayIcon.Icon = IconGenerator.CreateAppIcon();
        _trayIcon.DoubleClick += (s, e) =>
        {
            _mainForm.Show();
            _mainForm.BringToFront();
        };
    }

    public void UpdateTooltip(int windowCount)
    {
        _trayIcon.Text = windowCount == 0
            ? "XI Focus — No FFXI windows"
            : $"XI Focus — {windowCount} window{(windowCount == 1 ? "" : "s")} active";
    }

    public void Dispose()
    {
        _trayIcon.Dispose();
        _trayMenu.Dispose();
    }
}
