using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.SimpleLocalization.Scripts
{
    public static class LocalizationManager
    {
        public static event Action OnLocalizationChanged = () => { };

        public static Dictionary<string, Dictionary<string, string>> Dictionary = new();
        private static string _language = "English";

        public static string Language
        {
            get => _language;
            set { _language = value; OnLocalizationChanged(); }
        }

        public static void AutoLanguage()
        {
            Language = "English";
        }

        public static void Read()
        {
            if (Dictionary.Count > 0) return;

            var keys = new List<string>();

            foreach (var sheet in LocalizationSettings.Instance.Sheets)
            {
                if (sheet.TextAsset == null)
                {
                    continue;
                }

                try
                {
                    var lines = GetLines(sheet.TextAsset.text);
                    var languages = lines[0].Split(',').Select(i => i.Trim()).ToList();

                    if (languages.Count != languages.Distinct().Count())
                    {
                        Debug.LogError($"Duplicated languages found in `{sheet.Name}`. This sheet is not loaded.");
                        continue;
                    }

                    for (var i = 1; i < languages.Count; i++)
                    {
                        if (!Dictionary.ContainsKey(languages[i]))
                        {
                            Dictionary.Add(languages[i], new Dictionary<string, string>());
                        }
                    }

                    for (var i = 1; i < lines.Count; i++)
                    {
                        var columns = GetColumns(lines[i]);
                        var key = columns[0];

                        if (string.IsNullOrEmpty(key)) continue;

                        if (keys.Contains(key))
                        {
                            Debug.LogError($"Duplicated key `{key}` found in `{sheet.Name}`. This key is not loaded.");
                            continue;
                        }

                        keys.Add(key);

                        for (var j = 1; j < languages.Count; j++)
                        {
                            if (Dictionary[languages[j]].ContainsKey(key))
                            {
                                Debug.LogError($"Duplicated key `{key}` in `{sheet.Name}`.");
                            }
                            else
                            {
                                Dictionary[languages[j]].Add(key, columns[j]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"An error occurred while reading sheet '{sheet.Name}': {e.Message}. Skipping this sheet.");
                }
            }

            AutoLanguage();
        }

        public static bool HasKey(string localizationKey)
        {
            return Dictionary.ContainsKey(Language) && Dictionary[Language].ContainsKey(localizationKey);
        }

        public static string Localize(string localizationKey)
        {
            if (Dictionary.Count == 0)
            {
                Read();
            }

            if (!Dictionary.ContainsKey(Language)) throw new KeyNotFoundException("Language not found: " + Language);

            var missed = !Dictionary[Language].ContainsKey(localizationKey) || string.IsNullOrEmpty(Dictionary[Language][localizationKey]);

            if (missed)
            {
                Debug.LogWarning($"Translation not found: {localizationKey} ({Language}).");

                return Dictionary["English"].ContainsKey(localizationKey) ? Dictionary["English"][localizationKey] : localizationKey;
            }

            return Dictionary[Language][localizationKey];
        }

        public static string Localize(string localizationKey, params object[] args)
        {
            var pattern = Localize(localizationKey);

            return string.Format(pattern, args);
        }

        public static List<string> GetLines(string text)
        {
            text = text.Replace("\r\n", "\n").Replace("\"\"", "[_quote_]");

            var matches = Regex.Matches(text, "\"[\\s\\S]+?\"");

            foreach (Match match in matches)
            {
                text = text.Replace(match.Value, match.Value.Replace("\"", null).Replace(",", "[_comma_]").Replace("\n", "[_newline_]"));
            }

            // Making uGUI line breaks work in Asian texts.
            text = text.Replace("。", "。 ").Replace("、", "、 ").Replace("：", "： ").Replace("！", "！ ").Replace("（", " （").Replace("）", "） ").Trim();

            return text.Split('\n').Where(i => !string.IsNullOrEmpty(i)).ToList();
        }

        public static List<string> GetColumns(string line)
        {
            return line.Split(',').Select(j => j.Trim())
                .Select(j => j.Replace("[_quote_]", "\"").Replace("[_comma_]", ",").Replace("[_newline_]", "\n")).ToList();
        }
    }
}
