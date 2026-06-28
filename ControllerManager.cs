using SharpDX.XInput;

namespace XIFocus;

internal class ControllerManager : IDisposable
{
    private readonly Controller _controller;
    private readonly System.Threading.Timer _pollTimer;
    private readonly GamepadButtonFlags? _focusNextButton;
    private readonly GamepadButtonFlags? _focusPrevButton;
    private readonly List<GamepadButtonFlags?> _slotButtons;
    private GamepadButtonFlags _lastButtons;

    public volatile bool FocusNextPending = false;
    public volatile bool FocusPrevPending = false;
    public volatile int FocusSlotPending = -1;

    public ControllerManager(GamepadButtonFlags? focusNext, GamepadButtonFlags? focusPrev,
        List<GamepadButtonFlags?> slotButtons)
    {
        _focusNextButton = focusNext;
        _focusPrevButton = focusPrev;
        _slotButtons = slotButtons;

        _controller = new Controller(UserIndex.One);
        _lastButtons = 0;

        _pollTimer = new System.Threading.Timer(Poll, null, 0, 50);
    }

    public bool IsConnected => _controller.IsConnected;

    private void Poll(object? state)
    {
        if (!_controller.IsConnected)
        {
            return;
        }

        _controller.GetState(out var gamepadState);
        var current = gamepadState.Gamepad.Buttons;

        var pressed = current & ~_lastButtons;

        if (_focusNextButton.HasValue && pressed.HasFlag(_focusNextButton.Value))
        {
            FocusNextPending = true;
        }

        if (_focusPrevButton.HasValue && pressed.HasFlag(_focusPrevButton.Value))
        {
            FocusPrevPending = true;
        }

        for (int i = 0; i < _slotButtons.Count; i++)
        {
            if (_slotButtons[i].HasValue && pressed.HasFlag(_slotButtons[i]!.Value))
            {
                FocusSlotPending = i;
            }
        }

        if (pressed != 0)
        {
            Logger.Log($"[Controller] Button pressed: {pressed} — setting FocusNextPending={FocusNextPending}");
        }

        _lastButtons = current;
    }

    public void Dispose()
    {
        _pollTimer.Dispose();
    }
}
