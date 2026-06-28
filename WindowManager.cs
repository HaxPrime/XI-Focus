using System.Text;

namespace XIFocus;

internal class WindowManager
{
    private const int MaxClassNameLength = 256;

    private readonly List<IntPtr> _windows;
    private readonly List<string> _windowTitles;
    private int _currentIndex;
    private readonly Action? _onWindowsChanged;
    private WinApi.WinEventDelegate? _winEventProc;
    private IntPtr _winEventHook;
    private System.Threading.Timer? _debounceTimer;
    private readonly object _debounceLock = new();

    public WindowManager(Action? onWindowsChanged = null)
    {
        _windows = new List<IntPtr>();
        _windowTitles = new List<string>();
        _currentIndex = 0;
        _onWindowsChanged = onWindowsChanged;
        Refresh();
    }

    public void StartWatching()
    {
        _winEventProc = OnWinEvent;
        _winEventHook = WinApi.SetWinEventHook(
            0x0001,     // EVENT_MIN
            0x9999,     // EVENT_MAX — catch everything, filter in callback
            IntPtr.Zero, _winEventProc, 0, 0, WinApi.WINEVENT_OUTOFCONTEXT);
    }

    public void StopWatching()
    {
        if (_winEventHook != IntPtr.Zero)
        {
            WinApi.UnhookWinEvent(_winEventHook);
            _winEventHook = IntPtr.Zero;
        }
    }

    private void OnWinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hwnd,
        int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (idObject != 0) return; // OBJID_WINDOW = 0
        if (hwnd == IntPtr.Zero) return;

        var sb = new StringBuilder(256);
        WinApi.GetClassName(hwnd, sb, 256);
        if (sb.ToString() != "FFXiClass") return;

        lock (_debounceLock)
        {
            _debounceTimer?.Dispose();
            _debounceTimer = new System.Threading.Timer(_ =>
            {
                _onWindowsChanged?.Invoke();
            }, null, 500, Timeout.Infinite);
        }
    }

    public int WindowCount => _windows.Count;

    public int CurrentIndex => _currentIndex;

    public int Refresh()
    {
        // Remove windows that are gone or no longer FFXiClass
        for (int i = _windows.Count - 1; i >= 0; i--)
        {
            var sb = new StringBuilder(MaxClassNameLength);
            WinApi.GetClassName(_windows[i], sb, MaxClassNameLength);
            if (sb.ToString() != "FFXiClass" || WinApi.GetParent(_windows[i]) != IntPtr.Zero)
            {
                _windows.RemoveAt(i);
                _windowTitles.RemoveAt(i);
                if (_currentIndex >= _windows.Count)
                    _currentIndex = Math.Max(0, _windows.Count - 1);
            }
        }

        // Append new windows not yet tracked
        WinApi.EnumWindows((hwnd, _) =>
        {
            if (WinApi.GetParent(hwnd) != IntPtr.Zero) return true;
            var sb = new StringBuilder(MaxClassNameLength);
            WinApi.GetClassName(hwnd, sb, MaxClassNameLength);
            if (sb.ToString() != "FFXiClass") return true;
            if (_windows.Contains(hwnd)) return true;

            int len = WinApi.GetWindowTextLength(hwnd);
            var title = new StringBuilder(len + 1);
            WinApi.GetWindowText(hwnd, title, title.Capacity);

            _windows.Add(hwnd);
            _windowTitles.Add(title.ToString());
            return true;
        }, IntPtr.Zero);

        // Re-check titles for windows still showing a generic name
        for (int i = 0; i < _windows.Count; i++)
        {
            if (string.IsNullOrEmpty(_windowTitles[i]) ||
                _windowTitles[i] == "FINAL FANTASY XI" ||
                _windowTitles[i] == "Final Fantasy XI")
            {
                int len = WinApi.GetWindowTextLength(_windows[i]);
                var title = new StringBuilder(len + 1);
                WinApi.GetWindowText(_windows[i], title, title.Capacity);
                string newTitle = title.ToString();
                if (!string.IsNullOrEmpty(newTitle))
                    _windowTitles[i] = newTitle;
            }
        }

        return _windows.Count;
    }

    public string GetTitle(int index)
    {
        if (index < 0 || index >= _windowTitles.Count)
            return $"Slot {index + 1}";
        string t = _windowTitles[index];
        if (string.IsNullOrEmpty(t) || t == "FINAL FANTASY XI" || t == "Final Fantasy XI")
            return $"Slot {index + 1} (logging in...)";
        return t;
    }

    public bool MoveUp(int index)
    {
        if (index <= 0 || index >= _windows.Count) return false;
        (_windows[index], _windows[index - 1]) = (_windows[index - 1], _windows[index]);
        (_windowTitles[index], _windowTitles[index - 1]) = (_windowTitles[index - 1], _windowTitles[index]);
        if (_currentIndex == index) _currentIndex--;
        else if (_currentIndex == index - 1) _currentIndex++;
        return true;
    }

    public bool MoveDown(int index)
    {
        if (index < 0 || index >= _windows.Count - 1) return false;
        (_windows[index], _windows[index + 1]) = (_windows[index + 1], _windows[index]);
        (_windowTitles[index], _windowTitles[index + 1]) = (_windowTitles[index + 1], _windowTitles[index]);
        if (_currentIndex == index) _currentIndex++;
        else if (_currentIndex == index + 1) _currentIndex--;
        return true;
    }

    public List<WindowLayout> CaptureLayout()
    {
        var layout = new List<WindowLayout>();
        for (int i = 0; i < _windows.Count; i++)
        {
            WinApi.GetWindowRect(_windows[i], out var rect);
            layout.Add(new WindowLayout
            {
                CharacterName = _windowTitles[i],
                X = rect.Left,
                Y = rect.Top,
                Width = rect.Right - rect.Left,
                Height = rect.Bottom - rect.Top
            });
        }
        Logger.Log($"[Layout] Captured {layout.Count} window positions");
        return layout;
    }

    public void RestoreLayout(List<WindowLayout> layout)
    {
        foreach (var saved in layout)
        {
            int idx = _windowTitles.IndexOf(saved.CharacterName);
            if (idx < 0) continue;
            WinApi.SetWindowPos(
                _windows[idx], IntPtr.Zero,
                saved.X, saved.Y, saved.Width, saved.Height,
                WinApi.SWP_NOZORDER | WinApi.SWP_NOACTIVATE);
            Logger.Log($"[Layout] Restored {saved.CharacterName} to {saved.X},{saved.Y} {saved.Width}x{saved.Height}");
            System.Threading.Thread.Sleep(50);
            WinApi.RedrawWindow(_windows[idx], IntPtr.Zero, IntPtr.Zero,
                WinApi.RDW_INVALIDATE | WinApi.RDW_UPDATENOW | WinApi.RDW_ALLCHILDREN);
        }
    }

    public void FocusSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _windows.Count) return;
        _currentIndex = slotIndex;
        Logger.Log($"[FocusSlot] Jumped to slot {slotIndex} hWnd={_windows[_currentIndex]}");
        ForceForeground(_windows[_currentIndex]);
    }

    public void FocusNext()
    {
        if (_windows.Count == 0)
        {
            return;
        }

        _currentIndex++;
        if (_currentIndex >= _windows.Count)
        {
            _currentIndex = 0;
        }

        Logger.Log($"[FocusNext] Switching to index={_currentIndex} hWnd={_windows[_currentIndex]}");

        ForceForeground(_windows[_currentIndex]);
    }

    public void FocusPrevious()
    {
        if (_windows.Count == 0)
        {
            return;
        }

        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = _windows.Count - 1;
        }

        Logger.Log($"[FocusPrevious] Switching to index={_currentIndex} hWnd={_windows[_currentIndex]}");

        ForceForeground(_windows[_currentIndex]);
    }

    private void ForceForeground(IntPtr hWnd)
    {
        Logger.Log($"[ForceForeground] Called for hWnd={hWnd}");

        if (WinApi.IsIconic(hWnd))
        {
            WinApi.ShowWindow(hWnd, WinApi.ShowWindowFlags.SW_RESTORE);
            Logger.Log($"[ForceForeground] Restored from minimized");
        }

        IntPtr foregroundHwnd = WinApi.GetForegroundWindow();
        uint foregroundThread = WinApi.GetWindowThreadProcessId(foregroundHwnd, out _);
        uint currentThread = WinApi.GetCurrentThreadId();

        Logger.Log($"[ForceForeground] foregroundThread={foregroundThread} currentThread={currentThread}");

        bool attached = false;
        try
        {
            if (foregroundThread != currentThread)
            {
                attached = WinApi.AttachThreadInput(foregroundThread, currentThread, true);
                Logger.Log($"[ForceForeground] Attached to foreground thread: {attached}");
            }

            WinApi.BringWindowToTop(hWnd);
            WinApi.ShowWindow(hWnd, WinApi.ShowWindowFlags.SW_SHOW);
            Logger.Log($"[ForceForeground] BringWindowToTop+ShowWindow done");
        }
        finally
        {
            if (attached)
            {
                WinApi.AttachThreadInput(foregroundThread, currentThread, false);
                Logger.Log($"[ForceForeground] Detached");
            }
        }

        WinApi.SetForegroundWindow(hWnd);
        Logger.Log($"[ForceForeground] SetForegroundWindow called");
    }
}
