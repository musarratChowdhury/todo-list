using System.IO;
using System.Text.Json;

namespace TodoList;

public enum DockedSize { Small, Medium, Large }

public class AppSettings
{
    public DockedSize DockedWindowSize { get; set; } = DockedSize.Medium;

    public string HighColor { get; set; } = "#A5D6A7";
    public string MedColor { get; set; } = "#C8E6C9";
    public string LowColor { get; set; } = "#E8F5E9";

    private const string SettingsFile = "settings.json";
    private static AppSettings? _instance;

    public static AppSettings Instance => _instance ??= Load();

    private static AppSettings Load()
    {
        if (File.Exists(SettingsFile))
        {
            try
            {
                var json = File.ReadAllText(SettingsFile);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch { }
        }
        return new AppSettings();
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsFile, json);
    }

    /// <summary>Returns (Width, Height, FontSize) for the docked mini window.</summary>
    public (double Width, double Height, double FontSize) GetDockedDimensions() =>
        DockedWindowSize switch
        {
            DockedSize.Small  => (170, 44, 17),
            DockedSize.Large  => (290, 68, 28),
            _                 => (220, 56, 22),  // Medium (default)
        };
}
