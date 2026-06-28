using SharpDX.XInput;

namespace XIFocus;

public partial class BindKeyForm : Form
{
    private readonly Label _lblAction;
    private readonly Label _lblPrompt;
    private readonly Label _lblEsc;
    private readonly Label _lblDetectedHeader;
    private readonly Label _lblDetected;
    private readonly Button _btnConfirm;
    private readonly Button _btnTryAgain;
    private readonly System.Windows.Forms.Timer _controllerTimer;

    private bool _confirming = false;

    public Keys? BoundKey { get; private set; } = null;
    public GamepadButtonFlags? BoundButton { get; private set; } = null;

    public BindKeyForm(string actionName)
    {
        Text = "XI Focus — Bind Key";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Size = new Size(320, 220);
        StartPosition = FormStartPosition.CenterParent;
        KeyPreview = true;

        _lblAction = new Label
        {
            Text = actionName,
            Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(280, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(_lblAction);

        _lblPrompt = new Label
        {
            Text = "Press any key or controller button...",
            Location = new Point(10, 70),
            Size = new Size(280, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(_lblPrompt);

        _lblEsc = new Label
        {
            Text = "Esc to cancel",
            Font = new Font(Font.FontFamily, 8),
            ForeColor = Color.Gray,
            Location = new Point(10, 150),
            Size = new Size(280, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(_lblEsc);

        _lblDetectedHeader = new Label
        {
            Text = "Detected:",
            Location = new Point(10, 55),
            Size = new Size(280, 20),
            TextAlign = ContentAlignment.MiddleCenter,
            Visible = false
        };
        Controls.Add(_lblDetectedHeader);

        _lblDetected = new Label
        {
            Text = string.Empty,
            Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
            Location = new Point(10, 80),
            Size = new Size(280, 30),
            TextAlign = ContentAlignment.MiddleCenter,
            Visible = false
        };
        Controls.Add(_lblDetected);

        _btnConfirm = new Button
        {
            Text = "Confirm",
            Location = new Point(160, 140),
            Size = new Size(90, 30),
            DialogResult = DialogResult.OK,
            Visible = false
        };
        Controls.Add(_btnConfirm);

        _btnTryAgain = new Button
        {
            Text = "Try Again",
            Location = new Point(60, 140),
            Size = new Size(90, 30),
            Visible = false
        };
        _btnTryAgain.Click += BtnTryAgain_Click;
        Controls.Add(_btnTryAgain);

        _controllerTimer = new System.Windows.Forms.Timer { Interval = 50 };
        _controllerTimer.Tick += PollController;
        _controllerTimer.Start();

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
        }

        // Confirm is primary action — Steam green
        _btnConfirm.BackColor = Color.FromArgb(76, 107, 34);
        _btnConfirm.FlatAppearance.BorderColor = Color.FromArgb(76, 107, 34);
        _btnConfirm.FlatAppearance.MouseOverBackColor = Color.FromArgb(106, 150, 47);

        // Small Esc label stays muted
        _lblEsc.ForeColor = Color.FromArgb(143, 152, 160);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (_confirming)
        {
            return;
        }

        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        BoundKey = e.KeyCode;
        BoundButton = null;
        _lblDetected.Text = FriendlyKey(e.KeyCode);
        ShowConfirmPhase();
        e.Handled = true;
    }

    private void PollController(object? sender, EventArgs e)
    {
        if (_confirming)
        {
            return;
        }

        var controller = new Controller(UserIndex.One);
        if (!controller.IsConnected)
        {
            return;
        }

        controller.GetState(out var state);
        var buttons = state.Gamepad.Buttons;
        if (buttons == 0)
        {
            return;
        }

        foreach (GamepadButtonFlags flag in Enum.GetValues(typeof(GamepadButtonFlags)))
        {
            if (flag == GamepadButtonFlags.None)
            {
                continue;
            }

            if (buttons.HasFlag(flag))
            {
                BoundButton = flag;
                BoundKey = null;
                _lblDetected.Text = FriendlyButton(flag);
                ShowConfirmPhase();
                return;
            }
        }
    }

    private void ShowConfirmPhase()
    {
        _confirming = true;
        _lblPrompt.Visible = false;
        _lblEsc.Visible = false;
        _lblDetectedHeader.Visible = true;
        _lblDetected.Visible = true;
        _btnConfirm.Visible = true;
        _btnTryAgain.Visible = true;
    }

    private void BtnTryAgain_Click(object? sender, EventArgs e)
    {
        _confirming = false;
        BoundKey = null;
        BoundButton = null;
        _lblDetectedHeader.Visible = false;
        _lblDetected.Visible = false;
        _btnConfirm.Visible = false;
        _btnTryAgain.Visible = false;
        _lblPrompt.Visible = true;
        _lblEsc.Visible = true;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _controllerTimer.Stop();
        _controllerTimer.Dispose();
        base.OnFormClosed(e);
    }

    internal static string FriendlyKey(Keys k) => k switch
    {
        Keys.F1 => "F1",
        Keys.F2 => "F2",
        Keys.F3 => "F3",
        Keys.F4 => "F4",
        Keys.F5 => "F5",
        Keys.F6 => "F6",
        Keys.F7 => "F7",
        Keys.F8 => "F8",
        Keys.F9 => "F9",
        Keys.F10 => "F10",
        Keys.F11 => "F11",
        Keys.F12 => "F12",
        Keys.Scroll => "Scroll Lock",
        Keys.Pause => "Pause/Break",
        Keys.Insert => "Insert",
        Keys.Delete => "Delete",
        Keys.Home => "Home",
        Keys.End => "End",
        Keys.PageUp => "Page Up",
        Keys.PageDown => "Page Down",
        _ => k.ToString()
    };

    internal static string FriendlyButton(GamepadButtonFlags b) => b switch
    {
        GamepadButtonFlags.Start => "Start",
        GamepadButtonFlags.Back => "Back / Select",
        GamepadButtonFlags.A => "A",
        GamepadButtonFlags.B => "B",
        GamepadButtonFlags.X => "X",
        GamepadButtonFlags.Y => "Y",
        GamepadButtonFlags.LeftShoulder => "Left Bumper",
        GamepadButtonFlags.RightShoulder => "Right Bumper",
        GamepadButtonFlags.LeftThumb => "Left Stick Click",
        GamepadButtonFlags.RightThumb => "Right Stick Click",
        _ => b.ToString()
    };
}
