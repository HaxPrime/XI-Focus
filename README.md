# XI Focus

**FFXI instance switcher for Windows**

Switch focus between multiple Final Fantasy XI game windows using keyboard hotkeys or an Xbox controller - instantly, reliably, from any window.

---

## Features

- **Keyboard & controller switching** - bind any key or Xbox button to switch instances
- **Jump to slot** - bind keys/buttons to jump directly to a specific character (up to 6)
- **Auto-detection** - FFXI windows are detected and named automatically as characters log in
- **Slot reordering** - arrange your character order however you like
- **Named layouts** - save and restore window positions for different party configurations (Solo, Full Party, etc.)
- **Launch on startup** - optional Windows startup via Task Scheduler (no UAC prompt on boot)
- **System tray** - runs quietly in the background, right-click for quick access

---

## Requirements

- Windows 10 or Windows 11
- FFXI launched through POL or Ashita
- .NET 9 Runtime *(only if using the source build - the release exe is self-contained)*
- Xbox controller *(optional)*

---

## Installation

### Option A - Download the release exe *(recommended)*

1. Go to the [Releases](https://github.com/HaxPrime/XI-Focus/releases) page
2. Download `XIFocus.exe`
3. Place it anywhere you like - no installer needed
4. Right-click -> **Run as administrator**

> **SmartScreen warning:** Windows may show "Windows protected your PC" on first launch because the exe is unsigned. Click **More info -> Run anyway**. This is expected for community tools. You can verify the build hash in the About dialog (right-click tray icon -> About XI Focus).

### Option B - Build from source

Requirements: [.NET 9 SDK](https://dotnet.microsoft.com/download) and Git.

```powershell
git clone https://github.com/HaxPrime/XI-Focus.git
cd XI-Focus
dotnet build
```

Run the built exe as administrator:
```powershell
Start-Process "bin\Debug\net9.0-windows\XIFocus.exe" -Verb RunAs
```

To build a self-contained release exe:
```powershell
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

---

## Why Administrator?

FFXI launched through POL or Ashita runs as an elevated process. Windows UIPI (User Interface Privilege Isolation) silently blocks focus switching from lower-privilege applications - `SetForegroundWindow` simply does nothing without matching elevation.

XI Focus requests administrator privileges on launch so focus switching works reliably. This is the correct solution, not a workaround.

---

## First Time Setup

### 1. Launch your FFXI instances first
Start POL/Ashita and log in your characters before opening XI Focus. The app auto-detects windows as they appear.

### 2. Run XI Focus as administrator
Right-click the exe -> Run as administrator, or accept the UAC prompt if you have startup configured.

### 3. Your characters appear automatically
The slot list populates as each character logs in. No manual refresh needed.

### 4. Configure your bindings
Right-click the tray icon -> **Options** to bind keys and controller buttons.

Default bindings:
| Action | Keyboard | Controller |
|--------|----------|------------|
| Focus Next | F11 | Start |
| Focus Prev | F10 | Back / Select |

---

## Controller Setup

If all your FFXI instances are receiving controller input simultaneously, you need to disable gamepad passthrough in Ashita/Windower:

**Ashita:** In your Ashita config, disable gamepad input when the window is not focused.

**Windower:** Options -> Gamepad -> uncheck **"Always Enable Gamepad"** and **"Enable gamepad when game is inactive"**.

XI Focus takes exclusive control of the controller for hotkey monitoring - FFXI should not also be reading it simultaneously.

---

## Saving Window Layouts

1. Arrange your FFXI windows how you want them
2. In XI Focus, select or create a layout in the **Layout** dropdown
3. Click **Save**
4. Any time your windows get shuffled, click **Restore** to snap them back

You can have multiple named layouts - one for solo play, one for full party, etc.

To restore your layout automatically on startup: Options -> **Restore window layout on startup**.

---

## Building a Release

```powershell
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

Output: `bin\Release\net9.0-windows\win-x64\publish\XIFocus.exe`

Single file, no .NET runtime required on the target machine.

---

## Technical Notes

- Detects FFXI windows by Win32 class name `FFXiClass` - no memory reading, no offset tables
- Character names read from window title - version-agnostic, works on any private server
- Uses `AttachThreadInput` + `SetForegroundWindow` for reliable focus switching
- Controller polling via XInput - Xbox and compatible controllers, no DirectInput compatibility issues
- Startup via Windows Task Scheduler (`/rl HIGHEST`) - elevated at logon without UAC prompt

---

## License

MIT - see [LICENSE](LICENSE) for details.

---

## Author

**HaxPrime** - [github.com/HaxPrime](https://github.com/HaxPrime)

*Built with C# / .NET 9 - SharpDX.XInput - Win32 P/Invoke*
