using System.Text.Json;
using SharpDX.XInput;

namespace XIFocus;

public class WindowLayout
{
    public string CharacterName { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class Settings
{
    private static readonly string _settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "XIFocus", "settings.json");

    public Keys? FocusNextKey { get; set; } = Keys.Pause;
    public Keys? FocusPrevKey { get; set; } = Keys.Scroll;
    public GamepadButtonFlags? FocusNextButton { get; set; } = GamepadButtonFlags.Start;
    public GamepadButtonFlags? FocusPrevButton { get; set; } = GamepadButtonFlags.Back;
    public bool StartMinimized { get; set; } = false;
    public List<Keys?> SlotKeys { get; set; } = new List<Keys?> { null, null, null, null, null, null };
    public List<GamepadButtonFlags?> SlotButtons { get; set; } = new List<GamepadButtonFlags?> { null, null, null, null, null, null };
    public Dictionary<string, List<WindowLayout>> Layouts { get; set; } = new();
    public string ActiveLayoutName { get; set; } = "";
    public bool RestoreLayoutOnStart { get; set; } = false;

    public static Settings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return new Settings();
        }

        try
        {
            string json = File.ReadAllText(_settingsPath);
            var s = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            while (s.SlotKeys.Count < 6) s.SlotKeys.Add(null);
            while (s.SlotButtons.Count < 6) s.SlotButtons.Add(null);
            return s;
        }
        catch
        {
            return new Settings();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
        File.WriteAllText(_settingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
