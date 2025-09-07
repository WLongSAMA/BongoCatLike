using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BongoCat_Like.Utilities
{
    public static class ConfigManager
    {
        private static string ConfigPath;

        static ConfigManager()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            ConfigPath = Path.Combine(appPath, "config.json");
            if (!HasReadWritePermission(ConfigPath))
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string appName = GlobalHelper.Name;
                ConfigPath = Path.Combine(appDataPath, appName, "config.json");
            }
        }

        public static AppConfig LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                    return new AppConfig();

                string dir = Path.GetDirectoryName(ConfigPath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string json = File.ReadAllText(ConfigPath);
                return JsonSerializer.Deserialize(json, AppConfigContext.Default.AppConfig) ?? new AppConfig();
            }
            catch
            {
                return new AppConfig();
            }
        }

        public static void SaveConfig(AppConfig config)
        {
            try
            {
                string configDir = Path.GetDirectoryName(ConfigPath)!;
                if (!Directory.Exists(configDir) && !string.IsNullOrWhiteSpace(configDir))
                    Directory.CreateDirectory(configDir);

                string json = JsonSerializer.Serialize(config, AppConfigContext.Default.AppConfig);
                File.WriteAllText(ConfigPath, RemoveUnicodeEscapes(json));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"配置文件保存错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 移除 Unicode 转义
        /// </summary>
        /// <param name="text">需要移除转义的字符串</param>
        /// <returns>移除转义后的字符串</returns>
        private static string RemoveUnicodeEscapes(string text)
        {
            return Regex.Replace(text, @"\\u[0-9a-fA-F]{4}", m =>
            {
                string hexValue = m.Value[2..];
                char unicodeChar = (char)int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
                return unicodeChar.ToString();
            });
        }

        /// <summary>
        /// 检查当前用户对文件是否具有读写权限
        /// </summary>
        /// <param name="filePath">文件完整路径</param>
        /// <returns>有读写权限返回 true，否则返回 flase</returns>
        private static bool HasReadWritePermission(string filePath)
        {
            try
            {
                bool noFile = false;
                if (!File.Exists(filePath))
                    noFile = true;
                using FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs.Close();
                if (noFile)
                    File.Delete(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class AppConfig
    {
        public int WindowLeft { get; set; } = 0;
        public int WindowTop { get; set; } = 0;
        public string SkinId { get; set; } = "0";
        public string HatId { get; set; } = "0";
        public string Language { get; set; } = Localization.GetSystemLang();
        public int RandomSkin { get; set; } = 0;
        public bool Autorun { get; set; } = false;
        public bool TaskbarIcon { get; set; } = false;
        public bool Flip { get; set; } = false;
        public int Zoom { get; set; } = 5;
        public bool DisableDrag { get; set; } = false;
        public bool Adsorption { get; set; } = false;
        public bool Topmost { get; set; } = false;
        public bool MousePenetration { get; set; } = false;
    }

    [JsonSerializable(typeof(AppConfig))]
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        PropertyNameCaseInsensitive = true)]
    public partial class AppConfigContext : JsonSerializerContext { }
}
