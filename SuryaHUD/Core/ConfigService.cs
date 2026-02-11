using System;
using System.IO;
using System.Text.Json;

namespace SuryaHUD.Core
{
    public class ConfigService : IConfigService
    {
        private const string ConfigFileName = "settings.json";
        public HudConfig Config { get; private set; }

        public ConfigService()
        {
            Config = new HudConfig();
            Load();
        }

        public void Load()
        {
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFileName);
                    var loadedConfig = JsonSerializer.Deserialize<HudConfig>(json);
                    if (loadedConfig != null)
                    {
                        Config = loadedConfig;
                    }
                }
                catch
                {
                    // Fallback to default if corrupted
                }
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFileName, json);
            }
            catch
            {
                // Handle save error (maybe log?)
            }
        }
    }
}
