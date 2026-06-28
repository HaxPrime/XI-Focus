using SharpDX.XInput;

namespace XIFocus;

public partial class MainForm : Form
{
    private readonly Settings _settings;
    private readonly TrayManager _tray;
    private readonly WindowManager _windowManager;
    private HotkeyManager _hotkeyManager;
    private ControllerManager _controllerManager;
    private readonly System.Windows.Forms.Timer _controllerPollTimer;

    private readonly ComboBox _cboLayouts;
    private readonly Button _btnSaveLayout;
    private readonly Button _btnRestoreLayout;
    private readonly Button _btnNewLayout;
    private readonly Button _btnDeleteLayout;

    public MainForm()
    {
        _settings = Settings.Load();

        InitializeComponent();
        _windowManager = new WindowManager(() => this.Invoke(RefreshWindows));
        _windowManager.StartWatching();
        _tray = new TrayManager(this);
        _hotkeyManager = new HotkeyManager(
            _settings.FocusNextKey, _settings.FocusPrevKey,
            () => { _windowManager.FocusNext(); UpdateSlotList(); },
            () => { _windowManager.FocusPrevious(); UpdateSlotList(); },
            _settings.SlotKeys,
            (i) => { _windowManager.FocusSlot(i); UpdateSlotList(); }
        );
        _controllerManager = new ControllerManager(
            _settings.FocusNextButton,
            _settings.FocusPrevButton,
            _settings.SlotButtons
        );

        _controllerPollTimer = new System.Windows.Forms.Timer { Interval = 50 };
        _controllerPollTimer.Tick += ControllerTick;
        _controllerPollTimer.Enabled = true;

        var lblLayout = new Label { Text = "Layout:", Location = new Point(12, 255), AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Left };
        _cboLayouts = new ComboBox { Location = new Point(66, 251), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList, Anchor = AnchorStyles.Top | AnchorStyles.Left };
        _btnSaveLayout = new Button { Text = "Save", Location = new Point(210, 250), Size = new Size(55, 26), Anchor = AnchorStyles.Top | AnchorStyles.Left };
        _btnRestoreLayout = new Button { Text = "Restore", Location = new Point(269, 250), Size = new Size(60, 26), Anchor = AnchorStyles.Top | AnchorStyles.Left };
        _btnNewLayout = new Button { Text = "New", Location = new Point(333, 250), Size = new Size(50, 26), Anchor = AnchorStyles.Top | AnchorStyles.Left };
        _btnDeleteLayout = new Button { Text = "Delete", Location = new Point(387, 250), Size = new Size(55, 26), Anchor = AnchorStyles.Top | AnchorStyles.Left };
        Controls.AddRange(new Control[] { lblLayout, _cboLayouts, _btnSaveLayout, _btnRestoreLayout, _btnNewLayout, _btnDeleteLayout });

        _slotList.Resize += (s, e) => _columnFfxiWindows.Width = _slotList.ClientSize.Width;
        _slotList.SelectedIndexChanged += (s, e) => UpdateMoveButtons();
        _btnRefresh.Click += (s, e) => RefreshWindows();
        _btnMoveUp.Click += (s, e) =>
        {
            int sel = _slotList.SelectedIndices.Count > 0 ? _slotList.SelectedIndices[0] : -1;
            if (sel > 0)
            {
                _windowManager.MoveUp(sel);
                UpdateSlotList();
                _slotList.Items[sel - 1].Selected = true;
                _slotList.Items[sel - 1].EnsureVisible();
            }
        };
        _btnMoveDown.Click += (s, e) =>
        {
            int sel = _slotList.SelectedIndices.Count > 0 ? _slotList.SelectedIndices[0] : -1;
            if (sel >= 0 && sel < _windowManager.WindowCount - 1)
            {
                _windowManager.MoveDown(sel);
                UpdateSlotList();
                _slotList.Items[sel + 1].Selected = true;
                _slotList.Items[sel + 1].EnsureVisible();
            }
        };
        _btnFocusNext.Click += (s, e) =>
        {
            _windowManager.FocusNext();
            UpdateSlotList();
        };

        _cboLayouts.SelectedIndexChanged += (s, e) =>
        {
            _settings.ActiveLayoutName = _cboLayouts.SelectedItem?.ToString() ?? "";
            UpdateLayoutButtons();
        };

        _btnNewLayout.Click += (s, e) =>
        {
            string? name = InputDialog.Show("Enter a name for this layout:", "New Layout", "My Layout");
            if (string.IsNullOrWhiteSpace(name)) return;
            if (_settings.Layouts.ContainsKey(name))
            {
                MessageBox.Show(this, "A layout with that name already exists.", "XI Focus",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _settings.Layouts[name] = _windowManager.CaptureLayout();
            _settings.ActiveLayoutName = name;
            _settings.Save();
            RefreshLayoutDropdown();
        };

        _btnSaveLayout.Click += (s, e) =>
        {
            string name = _cboLayouts.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(name)) return;
            _settings.Layouts[name] = _windowManager.CaptureLayout();
            _settings.ActiveLayoutName = name;
            _settings.Save();
            MessageBox.Show(this, $"Layout '{name}' saved.", "XI Focus",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        _btnRestoreLayout.Click += (s, e) =>
        {
            string name = _cboLayouts.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(name) || !_settings.Layouts.ContainsKey(name)) return;
            _settings.ActiveLayoutName = name;
            _settings.Save();
            _windowManager.RestoreLayout(_settings.Layouts[name]);
        };

        _btnDeleteLayout.Click += (s, e) =>
        {
            string name = _cboLayouts.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(name)) return;
            if (_settings.Layouts.Count <= 1)
            {
                MessageBox.Show(this, "You must have at least one layout.", "XI Focus",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = MessageBox.Show(this, $"Delete layout '{name}'?", "XI Focus",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result != DialogResult.Yes) return;
            _settings.Layouts.Remove(name);
            if (_settings.ActiveLayoutName == name)
                _settings.ActiveLayoutName = _settings.Layouts.Keys.First();
            _settings.Save();
            RefreshLayoutDropdown();
        };

        _btnOptions.Click += (s, e) =>
        {
            if (new OptionsForm(_settings).ShowDialog(this) == DialogResult.OK)
            {
                RebindManagers();
            }
        };

        UpdateSlotList();
        this.Icon = IconGenerator.CreateAppIcon();
        this.BackColor = Color.FromArgb(27, 40, 56);
        this.ForeColor = Color.FromArgb(198, 212, 223);
        this.Font = new Font("Segoe UI", 9f);
        _slotList.BackColor = Color.FromArgb(42, 71, 94);
        _slotList.ForeColor = Color.FromArgb(198, 212, 223);
        _slotList.BorderStyle = BorderStyle.None;
        // Secondary buttons — Steam dark
        foreach (var btn in new Button[] {
            _btnRefresh, _btnOptions,
            _btnSaveLayout, _btnRestoreLayout, _btnNewLayout, _btnDeleteLayout })
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(49, 98, 130);
            btn.ForeColor = Color.FromArgb(198, 212, 223);
            btn.FlatAppearance.BorderColor = Color.FromArgb(27, 40, 56);
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 130, 160);
        }

        // Primary button — Steam green
        _btnFocusNext.FlatStyle = FlatStyle.Flat;
        _btnFocusNext.BackColor = Color.FromArgb(76, 107, 34);
        _btnFocusNext.ForeColor = Color.FromArgb(198, 212, 223);
        _btnFocusNext.FlatAppearance.BorderColor = Color.FromArgb(76, 107, 34);
        _btnFocusNext.FlatAppearance.BorderSize = 1;
        _btnFocusNext.FlatAppearance.MouseOverBackColor = Color.FromArgb(106, 150, 47);

        _btnMoveUp.FlatStyle = FlatStyle.Flat;
        _btnMoveUp.BackColor = Color.FromArgb(42, 71, 94);
        _btnMoveUp.ForeColor = Color.FromArgb(143, 152, 160);
        _btnMoveUp.FlatAppearance.BorderColor = Color.FromArgb(42, 71, 94);
        _btnMoveUp.FlatAppearance.BorderSize = 0;
        _btnMoveUp.FlatAppearance.MouseOverBackColor = Color.FromArgb(102, 192, 244);

        _btnMoveDown.FlatStyle = FlatStyle.Flat;
        _btnMoveDown.BackColor = Color.FromArgb(42, 71, 94);
        _btnMoveDown.ForeColor = Color.FromArgb(143, 152, 160);
        _btnMoveDown.FlatAppearance.BorderColor = Color.FromArgb(42, 71, 94);
        _btnMoveDown.FlatAppearance.BorderSize = 0;
        _btnMoveDown.FlatAppearance.MouseOverBackColor = Color.FromArgb(102, 192, 244);

        // Column header text
        _slotList.Columns[0].Text = "Character";

        // Combobox
        _cboLayouts.BackColor = Color.FromArgb(42, 71, 94);
        _cboLayouts.ForeColor = Color.FromArgb(198, 212, 223);
        _cboLayouts.FlatStyle = FlatStyle.Flat;
        _slotList.HeaderStyle = ColumnHeaderStyle.None;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
    }

    private void RefreshLayoutDropdown()
    {
        _cboLayouts.Items.Clear();
        foreach (var name in _settings.Layouts.Keys)
            _cboLayouts.Items.Add(name);
        if (_cboLayouts.Items.Count > 0)
        {
            int idx = _cboLayouts.Items.IndexOf(_settings.ActiveLayoutName);
            _cboLayouts.SelectedIndex = idx >= 0 ? idx : 0;
            _settings.ActiveLayoutName = _cboLayouts.SelectedItem?.ToString() ?? "";
        }
        UpdateLayoutButtons();
    }

    private void UpdateLayoutButtons()
    {
        bool hasLayouts = _cboLayouts.Items.Count > 0;
        bool hasSelection = _cboLayouts.SelectedIndex >= 0;
        _btnSaveLayout.Enabled = hasSelection;
        _btnRestoreLayout.Enabled = hasSelection;
        _btnDeleteLayout.Enabled = hasLayouts && _cboLayouts.Items.Count > 1;
    }

    public void RefreshWindows()
    {
        _windowManager.Refresh();
        UpdateSlotList();
    }


    public Settings GetSettings() => _settings;

    public void RebindManagers()
    {
        _hotkeyManager?.Dispose();
        _controllerManager.Dispose();

        _hotkeyManager = new HotkeyManager(
            _settings.FocusNextKey, _settings.FocusPrevKey,
            () => { _windowManager.FocusNext(); UpdateSlotList(); },
            () => { _windowManager.FocusPrevious(); UpdateSlotList(); },
            _settings.SlotKeys,
            (i) => { _windowManager.FocusSlot(i); UpdateSlotList(); }
        );
        _controllerManager = new ControllerManager(
            _settings.FocusNextButton,
            _settings.FocusPrevButton,
            _settings.SlotButtons
        );
    }

    private void ControllerTick(object? sender, EventArgs e)
    {
        if (_controllerManager.FocusNextPending || _controllerManager.FocusPrevPending)
        {
            Logger.Log($"[Tick] FocusNextPending={_controllerManager.FocusNextPending} FocusPrevPending={_controllerManager.FocusPrevPending}");
        }

        if (_controllerManager.FocusSlotPending >= 0)
        {
            int slot = _controllerManager.FocusSlotPending;
            _controllerManager.FocusSlotPending = -1;
            _windowManager.FocusSlot(slot);
            UpdateSlotList();
        }
        else if (_controllerManager.FocusNextPending)
        {
            _controllerManager.FocusNextPending = false;
            _windowManager.FocusNext();
            UpdateSlotList();
            Logger.Log($"[Tick] FocusNext/Prev called, WindowCount={_windowManager.WindowCount}");
        }
        else if (_controllerManager.FocusPrevPending)
        {
            _controllerManager.FocusPrevPending = false;
            _windowManager.FocusPrevious();
            UpdateSlotList();
            Logger.Log($"[Tick] FocusNext/Prev called, WindowCount={_windowManager.WindowCount}");
        }
    }

    private void UpdateMoveButtons()
    {
        int sel = _slotList.SelectedIndices.Count > 0 ? _slotList.SelectedIndices[0] : -1;
        _btnMoveUp.Enabled = sel > 0;
        _btnMoveDown.Enabled = sel >= 0 && sel < _windowManager.WindowCount - 1;
    }

    private void UpdateSlotList()
    {
        _slotList.Items.Clear();

        string controllerStatus = " | Controller: " + (_controllerManager.IsConnected ? "Connected" : "Not detected");

        if (_windowManager.WindowCount == 0)
        {
            _statusLabel.Text = "No characters detected" + controllerStatus;
            _tray.UpdateTooltip(0);
            UpdateMoveButtons();
            return;
        }

        for (int i = 0; i < _windowManager.WindowCount; i++)
        {
            var item = new ListViewItem($"{i + 1}. {_windowManager.GetTitle(i)}");
            if (i == _windowManager.CurrentIndex)
            {
                item.Font = new Font(_slotList.Font, FontStyle.Bold);
            }

            _slotList.Items.Add(item);
        }

        _statusLabel.Text = $"{_windowManager.WindowCount} character(s) detected" + controllerStatus;
        _tray.UpdateTooltip(_windowManager.WindowCount);
        UpdateMoveButtons();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
        }
        else
        {
            _settings.Save();
            _windowManager.StopWatching();
            _hotkeyManager?.Dispose();
            _controllerManager.Dispose();
            _controllerPollTimer.Dispose();
            _tray.Dispose();
        }

        base.OnFormClosing(e);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_settings.Layouts.Count == 0)
        {
            _settings.Layouts["Default"] = new List<WindowLayout>();
            _settings.ActiveLayoutName = "Default";
        }
        RefreshLayoutDropdown();
        if (_settings.RestoreLayoutOnStart &&
            _settings.Layouts.TryGetValue(_settings.ActiveLayoutName, out var layout) &&
            layout.Count > 0)
        {
            _windowManager.RestoreLayout(layout);
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_settings.StartMinimized)
        {
            Hide();
        }
    }
}
