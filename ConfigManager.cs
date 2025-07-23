using Newtonsoft.Json;
using System;
using System.IO;

public static class ConfigManager
{
    private const string ConfigFilePath = "config.json";

    public static void SaveConfig(string location)
    {
        var config = new ConfigData { LastSaveLocation = location };
        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(ConfigFilePath, json);
    }

    public static string LoadConfig()
    {
        if (File.Exists(ConfigFilePath))
        {
            try
            {
                string json = File.ReadAllText(ConfigFilePath);
                var config = JsonConvert.DeserializeObject<ConfigData>(json);
                return config?.LastSaveLocation;
            }
            catch (Exception)
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}

public class ConfigData
{
    public string LastSaveLocation { get; set; }
}
