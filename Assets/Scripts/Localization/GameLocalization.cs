using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace StudentSurvival.Localization
{
    // Unity Localization owns the locales and tables. This facade only caches the
    // selected table and saves the player's choice; there is no second string database.
    public sealed class GameLocalization : MonoBehaviour
    {
        public const string TableName = "Game";
        public static readonly string[] Codes = { "en", "uk", "ru", "fr", "it", "de", "es" };
        public static readonly string[] NativeNames = { "English", "Українська", "Русский", "Français", "Italiano", "Deutsch", "Español" };
        private static GameLocalization instance;
        private static StringTable table;
        private static StringTable english;
        public static bool IsReady { get; private set; }
        public static string LanguageCode { get; private set; } = "en";
        public static event Action Changed;
        private Coroutine loading;
        private AsyncOperationHandle<StringTable> retainedEnglish;
        private const string Preference = "StudentSurvival.Language";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            instance = null;
            table = english = null;
            IsReady = false;
            LanguageCode = "en";
            Changed = null;
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            instance = new GameObject("Localization").AddComponent<GameLocalization>();
            DontDestroyOnLoad(instance.gameObject);
        }
        // Load the tables synchronously on start-up so the very first rendered frame already has
        // its texts. Previously labels stayed empty until the asynchronous load finished on later
        // frames, which in the Editor could wait until the Game view had focus. WebGL cannot block,
        // so it keeps the asynchronous path in Start.
        private void Awake() { LoadImmediately(); }
        private void LoadImmediately()
        {
#if !UNITY_WEBGL
            try
            {
                var initialization = LocalizationSettings.InitializationOperation;
                if (!initialization.IsDone) initialization.WaitForCompletion();
                if (initialization.Status != AsyncOperationStatus.Succeeded) return;
                Locale fallback = LocalizationSettings.AvailableLocales.GetLocale("en");
                if (fallback == null) return;
                var fallbackLoad = LocalizationSettings.StringDatabase.GetTableAsync(TableName, fallback);
                if (!fallbackLoad.IsDone) fallbackLoad.WaitForCompletion();
                if (fallbackLoad.Status != AsyncOperationStatus.Succeeded || fallbackLoad.Result == null) return;
                retainedEnglish = Addressables.ResourceManager.Acquire(fallbackLoad);
                english = retainedEnglish.Result;
                string preferred = PlayerPrefs.GetString(Preference, SystemCode());
                Locale selected = LocalizationSettings.AvailableLocales.GetLocale(preferred) ?? fallback;
                // Select before subscribing, so this start-up choice does not start a second, async load.
                if (LocalizationSettings.SelectedLocale != selected) LocalizationSettings.SelectedLocale = selected;
                var selectedLoad = LocalizationSettings.StringDatabase.GetTableAsync(TableName, selected);
                if (!selectedLoad.IsDone) selectedLoad.WaitForCompletion();
                table = selectedLoad.Status == AsyncOperationStatus.Succeeded && selectedLoad.Result != null ? selectedLoad.Result : english;
                LanguageCode = table.LocaleIdentifier.Code;
                LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
                IsReady = true;
                Changed?.Invoke();
            }
            catch (Exception error) { Debug.LogWarning("Synchronous localization start-up failed; loading asynchronously. " + error.Message); }
#endif
        }
        private IEnumerator Start()
        {
            if (IsReady) yield break;
            var initialization = LocalizationSettings.InitializationOperation;
            yield return initialization;
            if (initialization.Status != AsyncOperationStatus.Succeeded)
            { Debug.LogError("Localization initialization failed. Run Tools > Student Survival > Set up localization."); yield break; }
            Locale fallback = LocalizationSettings.AvailableLocales.GetLocale("en");
            if (fallback == null) { Debug.LogError("English locale is missing."); yield break; }
            var fallbackLoad = LocalizationSettings.StringDatabase.GetTableAsync(TableName, fallback);
            yield return fallbackLoad;
            if (fallbackLoad.Status != AsyncOperationStatus.Succeeded || fallbackLoad.Result == null)
            { Debug.LogError("Game string table is missing. Run the localization setup command."); yield break; }
            // Locale changes release the database's cached tables. Retain the
            // fallback explicitly so it remains valid across those releases.
            if (!retainedEnglish.IsValid()) retainedEnglish = Addressables.ResourceManager.Acquire(fallbackLoad);
            english = retainedEnglish.Result;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            string preferred = PlayerPrefs.GetString(Preference, SystemCode());
            Locale selected = LocalizationSettings.AvailableLocales.GetLocale(preferred) ?? fallback;
            if (LocalizationSettings.SelectedLocale == selected) OnLocaleChanged(selected);
            else LocalizationSettings.SelectedLocale = selected;
        }
        private static string SystemCode()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Ukrainian: return "uk";
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.Italian: return "it";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Spanish: return "es";
                default: return "en";
            }
        }
        public static void Select(string code)
        {
            if (!IsReady || Array.IndexOf(Codes, code) < 0) return;
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(code);
            if (locale == null) return;
            PlayerPrefs.SetString(Preference, code);
            PlayerPrefs.Save();
            LocalizationSettings.SelectedLocale = locale;
        }
        private void OnLocaleChanged(Locale locale)
        {
            table = english;
            if (loading != null) StopCoroutine(loading);
            loading = StartCoroutine(Load(locale));
        }
        private IEnumerator Load(Locale locale)
        {
            var operation = LocalizationSettings.StringDatabase.GetTableAsync(TableName, locale);
            yield return operation;
            bool succeeded = operation.Status == AsyncOperationStatus.Succeeded && operation.Result != null;
            table = succeeded ? operation.Result : english;
            if (!succeeded) Debug.LogError("Could not load the Game table for " + locale.Identifier.Code + "; using English.");
            LanguageCode = table.LocaleIdentifier.Code;
            IsReady = true;
            Changed?.Invoke();
        }
        public static string Get(string key, params object[] arguments)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            string value = table?.GetEntry(key)?.LocalizedValue;
            if (string.IsNullOrEmpty(value)) value = english?.GetEntry(key)?.LocalizedValue;
            if (value == null) return IsReady ? "[" + key + "]" : string.Empty;
            // Press Start 2P covers our seven languages; its minus glyph uses ASCII hyphen-minus.
            value = value.Replace('\u2212', '-');
            if (arguments == null || arguments.Length == 0) return value;
            CultureInfo culture = table != null ? table.LocaleIdentifier.CultureInfo : CultureInfo.InvariantCulture;
            return string.Format(culture ?? CultureInfo.InvariantCulture, value, arguments);
        }
        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            if (retainedEnglish.IsValid()) Addressables.Release(retainedEnglish);
            if (instance == this) instance = null;
        }
    }
}
