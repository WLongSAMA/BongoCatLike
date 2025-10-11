using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BongoCat_Like.Utilities
{
    public class Localization : INotifyPropertyChanged
    {
        private static readonly string langDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locales");
        private static JsonDocument? _currentStrings;
        
        public static event Action? LanguageChanged;

        public static Localization Instance { get; } = new Localization();

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string this[string key] => GetString(key);

        public static void Initialize()
        {
            LoadLanguage(GetSystemLang());
        }

        public static string GetSystemLang()
        {
            return CultureInfo.CurrentUICulture.Name;
        }

        public static Dictionary<string, string> GetLangList()
        {
            Dictionary<string, string> list = [];
            if (!Directory.Exists(langDir))
                return list;

            string[] files = Directory.GetFiles(langDir, "*.json");
            foreach (string file in files)
            {
                try
                {
                    string cultureName = Path.GetFileNameWithoutExtension(file);
                    CultureInfo cultureInfo = new(cultureName);
                    list.Add(cultureInfo.Name, cultureInfo.NativeName);
                }
                catch (Exception)
                {
                    // 忽略无效语言文件
                }
            }
            return list;
        }

        public static void LoadLanguage(string langCode)
        {
            string filePath = Path.Combine(langDir, $"{langCode}.json");
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(langDir, "en-US.json");
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("语言文件 en-US.json 不存在！");
                _currentStrings = null;
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                _currentStrings = JsonDocument.Parse(json);
                LanguageChanged?.Invoke();
                Instance.NotifyPropertyChanged(nameof(Instance));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"语言文件加载失败：{ex.Message}");
                _currentStrings = null;
            }
        }

        public static string GetString(string key, params object[] args)
        {
            if (_currentStrings == null)
                return $"#{key}#";

            JsonElement current = _currentStrings.RootElement;
            foreach (string part in key.Split('.'))
            {
                if (current.TryGetProperty(part, out JsonElement value))
                    current = value;
                else
                    return $"#{key}#";
            }

            if (current.ValueKind == JsonValueKind.String)
            {
                string? strValue = current.GetString();
                if (strValue == null)
                    return $"#{key}#";

                return args.Length > 0 ? string.Format(strValue, args) : strValue;
            }

            return $"#{key}#";
        }
    }
}