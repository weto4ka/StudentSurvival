using System;
using System.IO;
using StudentSurvival.Localization;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Localization;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace StudentSurvival.Editor
{
    // Runs only in edit mode, adds missing entries, and never overwrites a
    // translator's table edits. The same setup is available explicitly in Tools.
    [InitializeOnLoad]
    public sealed class LocalizationSetup : IPreprocessBuildWithReport
    {
        private const string Root = "Assets/Localization";
        private const string Source = Root + "/Translations.json";
        private const string Marker = Root + "/SetupVersion.txt";
        private const string Version = "1.6.1";
        public int callbackOrder { get { return -100; } }
        [Serializable] private class Catalog { public Entry[] entries; }
        [Serializable] private class Entry
        {
            public string key, en, uk, ru, fr, it, de, es;
            public string Value(string code)
            {
                switch (code) { case "uk": return uk; case "ru": return ru; case "fr": return fr;
                    case "it": return it; case "de": return de; case "es": return es; default: return en; }
            }
        }
        static LocalizationSetup() { EditorApplication.update += TrySetup; }
        private static void TrySetup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            EditorApplication.update -= TrySetup;
            if (!File.Exists(Marker) || File.ReadAllText(Marker).Trim() != Stamp())
            {
                try { Setup(); }
                catch (Exception error) { Debug.LogException(error); }
            }
        }
        private static string Stamp() { return Version + ":" + (File.Exists(Source) ? Hash128.Compute(File.ReadAllText(Source)).ToString() : "missing"); }
        [MenuItem("Tools/Student Survival/Set up localization")]
        public static void Setup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            { Debug.LogWarning("Stop Play Mode before setting up localization."); return; }
            if (!File.Exists(Source)) throw new FileNotFoundException("Translation catalog missing", Source);
            Catalog catalog = JsonUtility.FromJson<Catalog>(File.ReadAllText(Source));
            if (catalog?.entries == null) throw new InvalidDataException("Translation catalog has no entries.");
            Directory.CreateDirectory(Root + "/Locales");
            Directory.CreateDirectory(Root + "/Tables");
            AssetDatabase.Refresh();
            var settings = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                AssetDatabase.CreateAsset(settings, Root + "/LocalizationSettings.asset");
                LocalizationEditorSettings.ActiveLocalizationSettings = settings;
                settings.GetStartupLocaleSelectors().Add(new SpecificLocaleSelector { LocaleId = new LocaleIdentifier("en") });
            }
            foreach (string code in GameLocalization.Codes)
            {
                if (LocalizationEditorSettings.GetLocale(code) != null) continue;
                Locale locale = Locale.CreateLocale(code);
                AssetDatabase.CreateAsset(locale, Root + "/Locales/" + code + ".asset");
                LocalizationEditorSettings.AddLocale(locale);
            }
            StringTableCollection collection = LocalizationEditorSettings.GetStringTableCollection(GameLocalization.TableName);
            if (collection == null)
                collection = LocalizationEditorSettings.CreateStringTableCollection(GameLocalization.TableName, Root + "/Tables");
            foreach (string code in GameLocalization.Codes)
            {
                StringTable table = collection.GetTable(code) as StringTable;
                if (table == null) table = collection.AddNewTable(new LocaleIdentifier(code)) as StringTable;
                if (table == null) throw new InvalidDataException("Could not create Game table for " + code);
                foreach (Entry entry in catalog.entries)
                {
                    string value = entry.Value(code);
                    if (string.IsNullOrWhiteSpace(value)) throw new InvalidDataException("Empty translation: " + entry.key + "/" + code);
                    if (table.GetEntry(entry.key) == null) table.AddEntry(entry.key, value);
                }
                LocalizationEditorSettings.SetPreloadTableFlag(table, true);
                EditorUtility.SetDirty(table);
            }
            EditorUtility.SetDirty(collection.SharedData);
            EditorUtility.SetDirty(settings);
            // Localization uses Addressables. Include their data when the user
            // builds a player, independently of this machine's editor preferences.
            AddressableAssetSettings addressables = AddressableAssetSettingsDefaultObject.Settings;
            if (addressables != null)
            {
                addressables.BuildAddressablesWithPlayerBuild = AddressableAssetSettings.PlayerBuildOption.BuildWithPlayer;
                EditorUtility.SetDirty(addressables);
            }
            AssetDatabase.SaveAssets();
            File.WriteAllText(Marker, Stamp() + "\n");
            AssetDatabase.Refresh();
            Debug.Log("Student Survival localization set up: seven locales and Game tables. Existing translations preserved.");
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            Setup();
            if (LocalizationEditorSettings.ActiveLocalizationSettings == null)
                throw new BuildFailedException("Localization settings are missing.");
        }
    }
}
