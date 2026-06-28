using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XIFocus;

internal class HotkeyManager : IDisposable
{
    private readonly WinApi.LowLevelKeyboardProc _hookCallback;
    private readonly IntPtr _hookHandle;
    private readonly Keys? _focusNextKey;
    private readonly Keys? _focusPrevKey;
    private readonly Action _onFocusNext;
    private readonly Action _onFocusPrev;
    private readonly List<Keys?> _slotKeys;
    private readonly Action<int> _onFocusSlot;

    public HotkeyManager(Keys? focusNext, Keys? focusPrev, Action onFocusNext, Action onFocusPrev,
        List<Keys?> slotKeys, Action<int> onFocusSlot)
    {
        _focusNextKey = focusNext;
        _focusPrevKey = focusPrev;
        _onFocusNext = onFocusNext;
        _onFocusPrev = onFocusPrev;
        _slotKeys = slotKeys;
        _onFocusSlot = onFocusSlot;

        _hookCallback = HookProc;
        var module = WinApi.GetModuleHandle(Process.GetCurrentProcess().MainModule!.ModuleName);
        _hookHandle = WinApi.SetWindowsHookEx(WinApi.WH_KEYBOARD_LL, _hookCallback, module, 0);
    }

    private IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && (int)wParam == WinApi.WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);

            if (_focusNextKey.HasValue && vkCode == (int)_focusNextKey.Value)
            {
                _onFocusNext();
            }
            else if (_focusPrevKey.HasValue && vkCode == (int)_focusPrevKey.Value)
            {
                _onFocusPrev();
            }
            else
            {
                for (int i = 0; i < _slotKeys.Count; i++)
                {
                    if (_slotKeys[i].HasValue && vkCode == (int)_slotKeys[i]!.Value)
                    {
                        _onFocusSlot(i);
                        break;
                    }
                }
            }
        }

        return WinApi.CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        WinApi.UnhookWindowsHookEx(_hookHandle);
    }
}
