﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Leauge_Auto_Accept
{
    internal static class Strings
    {
        public static readonly List<(string Code, string Name)> AvailableLanguages = new();

        public static string CurrentLanguage = "en";

        private static readonly Dictionary<string, Dictionary<string, string>> translations = new();

        public static void LoadLanguages()
        {
            AvailableLanguages.Clear();
            translations.Clear();

            string languagesDir = Path.Combine(AppContext.BaseDirectory, "Languages");

            if (!Directory.Exists(languagesDir))
            {
                return;
            }

            foreach (var file in Directory.GetFiles(languagesDir, "*.json").OrderBy(f => f))
            {
                string code = Path.GetFileNameWithoutExtension(file);

                try
                {
                    string json = File.ReadAllText(file);
                    var raw = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (raw == null) continue;

                    string name = raw.TryGetValue("__name", out var languageName) ? languageName : code;
                    raw.Remove("__name");

                    translations[code] = raw;
                    AvailableLanguages.Add((code, name));
                }
                catch
                {
                }
            }

            var enIndex = AvailableLanguages.FindIndex(l => l.Code == "en");
            if (enIndex > 0)
            {
                var en = AvailableLanguages[enIndex];
                AvailableLanguages.RemoveAt(enIndex);
                AvailableLanguages.Insert(0, en);
            }
        }

        public static string Get(string key)
        {
            if (translations.TryGetValue(CurrentLanguage, out var dict) && dict.TryGetValue(key, out var value))
                return value;

            return translations.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var fallback)
                ? fallback
                : key;
        }

        public static string NameForCode(string code)
        {
            var match = AvailableLanguages.FirstOrDefault(l => l.Code == code);
            return match.Name ?? code;
        }
    }
}
