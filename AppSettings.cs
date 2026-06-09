using System;
using System.IO;
using System.Text.Json;

namespace JabraSwitcher
{
    public class AppSettings
    {
        public string DefaultOutput { get; set; }
        public string DefaultInput { get; set; }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { WriteIndented = true };

        private static string SettingsFileName => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JabraSwitcher", "settings.json");

        public void Save()
        {
            var dir = Path.GetDirectoryName(SettingsFileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(SettingsFileName, json);
        }

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsFileName)) return null;
                var json = File.ReadAllText(SettingsFileName);
                return JsonSerializer.Deserialize<AppSettings>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
