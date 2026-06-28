using SharpDX.XInput;

namespace XIFocus;

public partial class OptionsForm : Form
{
    private readonly Settings _settings;
    private readonly Label _lblFocusNextKeyValue;
    private readonly Label _lblFocusPrevKeyValue;
    private readonly Label _lblFocusNextButtonValue;
    private readonly Label _lblFocusPrevButtonValue;
    private readonly CheckBox _chkStartMinimized;
    private readonly CheckBox _chkRestoreLayoutOnStart;
    private readonly Label _lblStartupLayout;
    private readonly CheckBox _chkLaunchOnStartup;
    private readonly Button _btnSave;
    private readonly Button _btnCancel;
    private readonly List<Keys?> _pendingSlotKeys;
    private readonly List<GamepadButtonFlags?> _pendingSlotButtons;
    private readonly Label[] _lblSlotValues;

    private Keys? _pendingFocusNextKey;
    private Keys? _pendingFocusPrevKey;
    private GamepadButtonFlags? _pendingFocusNextButton;
    private GamepadButtonFlags? _pendingFocusPrevButton;

    public OptionsForm(Settings settings)
    {
        _settings = settings;

        _pendingFocusNextKey = _settings.FocusNextKey;
        _pendingFocusPrevKey = _settings.FocusPrevKey;
        _pendingFocusNextButton = _settings.FocusNextButton;
        _pendingFocusPrevButton = _settings.FocusPrevButton;
        _pendingSlotKeys = new List<Keys?>(_settings.SlotKeys);
        _pendingSlotButtons = new List<GamepadButtonFlags?>(_settings.SlotButtons);
        _lblSlotValues = new Label[6];

        Text = "XI Focus — Options";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Size = new Size(450, 640);
        StartPosition = FormStartPosition.CenterParent;

        const int labelX = 15;
        const int valueX = 150;
        const int valueWidth = 110;
        const int assignX = 270;
        const int assignWidth = 65;
        const int clearX = 345;
        const int clearWidth = 60;
        const int rowHeight = 35;
        int y = 15;

        Controls.Add(new Label { Text = "Focus Next Key:", Location = new Point(labelX, y + 3), AutoSize = true });
        _lblFocusNextKeyValue = new Label { Location = new Point(valueX, y + 3), Size = new Size(valueWidth, 20) };
        SetBindingLabel(_lblFocusNextKeyValue, _pendingFocusNextKey.HasValue ? BindKeyForm.FriendlyKey(_pendingFocusNextKey.Value) : null);
        Controls.Add(_lblFocusNextKeyValue);
        var btnAssignFocusNextKey = new Button { Text = "Assign", Location = new Point(assignX, y), Size = new Size(assignWidth, 26) };
        btnAssignFocusNextKey.Click += (_, _) => BindKey("Switch Next", _lblFocusNextKeyValue, v => _pendingFocusNextKey = v);
        Controls.Add(btnAssignFocusNextKey);
        var btnClearFocusNextKey = new Button { Text = "Clear", Location = new Point(clearX, y), Size = new Size(clearWidth, 26) };
        btnClearFocusNextKey.Click += (_, _) => { _pendingFocusNextKey = null; SetBindingLabel(_lblFocusNextKeyValue, null); };
        Controls.Add(btnClearFocusNextKey);
        y += rowHeight;

        Controls.Add(new Label { Text = "Focus Prev Key:", Location = new Point(labelX, y + 3), AutoSize = true });
        _lblFocusPrevKeyValue = new Label { Location = new Point(valueX, y + 3), Size = new Size(valueWidth, 20) };
        SetBindingLabel(_lblFocusPrevKeyValue, _pendingFocusPrevKey.HasValue ? BindKeyForm.FriendlyKey(_pendingFocusPrevKey.Value) : null);
        Controls.Add(_lblFocusPrevKeyValue);
        var btnAssignFocusPrevKey = new Button { Text = "Assign", Location = new Point(assignX, y), Size = new Size(assignWidth, 26) };
        btnAssignFocusPrevKey.Click += (_, _) => BindKey("Switch Previous", _lblFocusPrevKeyValue, v => _pendingFocusPrevKey = v);
        Controls.Add(btnAssignFocusPrevKey);
        var btnClearFocusPrevKey = new Button { Text = "Clear", Location = new Point(clearX, y), Size = new Size(clearWidth, 26) };
        btnClearFocusPrevKey.Click += (_, _) => { _pendingFocusPrevKey = null; SetBindingLabel(_lblFocusPrevKeyValue, null); };
        Controls.Add(btnClearFocusPrevKey);
        y += rowHeight;

        Controls.Add(new Label { Text = "Focus Next Button:", Location = new Point(labelX, y + 3), AutoSize = true });
        _lblFocusNextButtonValue = new Label { Location = new Point(valueX, y + 3), Size = new Size(valueWidth, 20) };
        SetBindingLabel(_lblFocusNextButtonValue, _pendingFocusNextButton.HasValue ? BindKeyForm.FriendlyButton(_pendingFocusNextButton.Value) : null);
        Controls.Add(_lblFocusNextButtonValue);
        var btnAssignFocusNextButton = new Button { Text = "Assign", Location = new Point(assignX, y), Size = new Size(assignWidth, 26) };
        btnAssignFocusNextButton.Click += (_, _) => BindButton("Switch Next", _lblFocusNextButtonValue, v => _pendingFocusNextButton = v);
        Controls.Add(btnAssignFocusNextButton);
        var btnClearFocusNextButton = new Button { Text = "Clear", Location = new Point(clearX, y), Size = new Size(clearWidth, 26) };
        btnClearFocusNextButton.Click += (_, _) => { _pendingFocusNextButton = null; SetBindingLabel(_lblFocusNextButtonValue, null); };
        Controls.Add(btnClearFocusNextButton);
        y += rowHeight;

        Controls.Add(new Label { Text = "Focus Prev Button:", Location = new Point(labelX, y + 3), AutoSize = true });
        _lblFocusPrevButtonValue = new Label { Location = new Point(valueX, y + 3), Size = new Size(valueWidth, 20) };
        SetBindingLabel(_lblFocusPrevButtonValue, _pendingFocusPrevButton.HasValue ? BindKeyForm.FriendlyButton(_pendingFocusPrevButton.Value) : null);
        Controls.Add(_lblFocusPrevButtonValue);
        var btnAssignFocusPrevButton = new Button { Text = "Assign", Location = new Point(assignX, y), Size = new Size(assignWidth, 26) };
        btnAssignFocusPrevButton.Click += (_, _) => BindButton("Switch Previous", _lblFocusPrevButtonValue, v => _pendingFocusPrevButton = v);
        Controls.Add(btnAssignFocusPrevButton);
        var btnClearFocusPrevButton = new Button { Text = "Clear", Location = new Point(clearX, y), Size = new Size(clearWidth, 26) };
        btnClearFocusPrevButton.Click += (_, _) => { _pendingFocusPrevButton = null; SetBindingLabel(_lblFocusPrevButtonValue, null); };
        Controls.Add(btnClearFocusPrevButton);
        y += rowHeight;

        _chkStartMinimized = new CheckBox
        {
            Text = "Start minimized to tray",
            Location = new Point(labelX, y),
            AutoSize = true,
            Checked = _settings.StartMinimized
        };
        Controls.Add(_chkStartMinimized);
        y += rowHeight;

        _chkRestoreLayoutOnStart = new CheckBox
        {
            Text = "Restore window layout on startup",
            Location = new Point(labelX, y),
            AutoSize = true,
            Checked = _settings.RestoreLayoutOnStart
        };
        Controls.Add(_chkRestoreLayoutOnStart);
        string activeName = _settings.ActiveLayoutName;
        bool hasLayout = _settings.Layouts.Count > 0 && !string.IsNullOrEmpty(activeName);
        _lblStartupLayout = new Label
        {
            Left = _chkRestoreLayoutOnStart.Right + 6,
            Top = _chkRestoreLayoutOnStart.Top + 3,
            AutoSize = true,
            ForeColor = Color.Gray,
            Font = new Font(Font, FontStyle.Italic),
            Text = hasLayout ? $"(using \"{activeName}\")" : "(no layout saved)"
        };
        Controls.Add(_lblStartupLayout);
        y += rowHeight;

        _chkLaunchOnStartup = new CheckBox
        {
            Text = "Launch XI Focus on Windows startup",
            Location = new Point(labelX, y),
            AutoSize = true,
            Checked = StartupManager.IsEnabled()
        };
        Controls.Add(_chkLaunchOnStartup);
        y += rowHeight;

        Controls.Add(new Label
        {
            Text = "Slot Jump Bindings:",
            Location = new Point(labelX, y + 3),
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold)
        });
        y += rowHeight - 5;

        for (int i = 0; i < 6; i++)
        {
            int slotIndex = i;
            Controls.Add(new Label { Text = $"Slot {i + 1}:", Location = new Point(labelX, y + 3), AutoSize = true });

            _lblSlotValues[i] = new Label { Location = new Point(valueX, y + 3), Size = new Size(valueWidth, 20) };
            string? slotDisplay = _pendingSlotKeys[i].HasValue
                ? BindKeyForm.FriendlyKey(_pendingSlotKeys[i]!.Value)
                : _pendingSlotButtons[i].HasValue
                    ? BindKeyForm.FriendlyButton(_pendingSlotButtons[i]!.Value)
                    : null;
            SetBindingLabel(_lblSlotValues[i], slotDisplay);
            Controls.Add(_lblSlotValues[i]);

            var btnAssignSlot = new Button { Text = "Assign", Location = new Point(assignX, y), Size = new Size(assignWidth, 26) };
            btnAssignSlot.Click += (_, _) => BindSlot(slotIndex, _lblSlotValues[slotIndex]);
            Controls.Add(btnAssignSlot);

            var btnClearSlot = new Button { Text = "Clear", Location = new Point(clearX, y), Size = new Size(clearWidth, 26) };
            btnClearSlot.Click += (_, _) =>
            {
                _pendingSlotKeys[slotIndex] = null;
                _pendingSlotButtons[slotIndex] = null;
                SetBindingLabel(_lblSlotValues[slotIndex], null);
            };
            Controls.Add(btnClearSlot);
            y += rowHeight;
        }

        y += 5;

        _btnSave = new Button
        {
            Text = "Save",
            Location = new Point(valueX + 95, y),
            Size = new Size(85, 28),
            DialogResult = DialogResult.OK
        };
        _btnSave.Click += BtnSave_Click;
        Controls.Add(_btnSave);

        _btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(valueX, y),
            Size = new Size(85, 28),
            DialogResult = DialogResult.Cancel
        };
        Controls.Add(_btnCancel);
        y += rowHeight;

        Controls.Add(new Label
        {
            Text = "Changes take effect after saving.",
            ForeColor = Color.Gray,
            Location = new Point(labelX, y),
            AutoSize = true
        });

        AcceptButton = _btnSave;
        CancelButton = _btnCancel;

        ApplyTheme();
    }

    private void ApplyTheme()
    {
        this.BackColor = Color.FromArgb(27, 40, 56);
        this.ForeColor = Color.FromArgb(198, 212, 223);
        this.Font = new Font("Segoe UI", 9f);

        foreach (Control c in this.Controls)
        {
            c.ForeColor = Color.FromArgb(198, 212, 223);

            if (c is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = Color.FromArgb(49, 98, 130);
                btn.ForeColor = Color.FromArgb(198, 212, 223);
                btn.FlatAppearance.BorderColor = Color.FromArgb(27, 40, 56);
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 130, 160);
            }

            if (c is Label lbl)
            {
                lbl.BackColor = Color.FromArgb(27, 40, 56);
            }

            if (c is CheckBox chk)
            {
                chk.BackColor = Color.FromArgb(27, 40, 56);
                chk.ForeColor = Color.FromArgb(198, 212, 223);
            }
        }

        // Save button is primary action — Steam green
        _btnSave.BackColor = Color.FromArgb(76, 107, 34);
        _btnSave.FlatAppearance.BorderColor = Color.FromArgb(76, 107, 34);
        _btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(106, 150, 47);
    }

    private void BindKey(string actionName, Label targetLabel, Action<Keys?> apply)
    {
        using var form = new BindKeyForm(actionName);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        if (form.BoundKey.HasValue)
        {
            apply(form.BoundKey.Value);
            SetBindingLabel(targetLabel, BindKeyForm.FriendlyKey(form.BoundKey.Value));
        }
        else if (!form.BoundButton.HasValue)
        {
            apply(null);
            SetBindingLabel(targetLabel, null);
        }
    }

    private void BindButton(string actionName, Label targetLabel, Action<GamepadButtonFlags?> apply)
    {
        using var form = new BindKeyForm(actionName);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        if (form.BoundButton.HasValue)
        {
            apply(form.BoundButton.Value);
            SetBindingLabel(targetLabel, BindKeyForm.FriendlyButton(form.BoundButton.Value));
        }
        else if (!form.BoundKey.HasValue)
        {
            apply(null);
            SetBindingLabel(targetLabel, null);
        }
    }

    private void BindSlot(int slotIndex, Label targetLabel)
    {
        using var form = new BindKeyForm($"Slot {slotIndex + 1}");
        if (form.ShowDialog(this) != DialogResult.OK) return;

        if (form.BoundKey.HasValue)
        {
            _pendingSlotKeys[slotIndex] = form.BoundKey.Value;
            _pendingSlotButtons[slotIndex] = null;
            SetBindingLabel(targetLabel, BindKeyForm.FriendlyKey(form.BoundKey.Value));
        }
        else if (form.BoundButton.HasValue)
        {
            _pendingSlotButtons[slotIndex] = form.BoundButton.Value;
            _pendingSlotKeys[slotIndex] = null;
            SetBindingLabel(targetLabel, BindKeyForm.FriendlyButton(form.BoundButton.Value));
        }
    }

    private static void SetBindingLabel(Label label, string? friendlyName)
    {
        if (friendlyName != null)
        {
            label.Text = friendlyName;
            label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Bold);
            label.ForeColor = SystemColors.ControlText;
        }
        else
        {
            label.Text = "(none)";
            label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Italic);
            label.ForeColor = Color.Gray;
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        _settings.FocusNextKey = _pendingFocusNextKey;
        _settings.FocusPrevKey = _pendingFocusPrevKey;
        _settings.FocusNextButton = _pendingFocusNextButton;
        _settings.FocusPrevButton = _pendingFocusPrevButton;
        _settings.StartMinimized = _chkStartMinimized.Checked;
        _settings.RestoreLayoutOnStart = _chkRestoreLayoutOnStart.Checked;
        if (_chkLaunchOnStartup.Checked)
            StartupManager.Enable(StartupManager.GetCurrentExePath());
        else
            StartupManager.Disable();
        for (int i = 0; i < 6; i++)
        {
            _settings.SlotKeys[i] = _pendingSlotKeys[i];
            _settings.SlotButtons[i] = _pendingSlotButtons[i];
        }
        _settings.Save();
        Close();
    }
}
