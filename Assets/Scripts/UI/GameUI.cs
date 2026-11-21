using System;
using System.Collections.Generic;
using StudentSurvival.Characters;
using StudentSurvival.Controls;
using StudentSurvival.Inventory;
using StudentSurvival.Localization;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StudentSurvival.UI
{
    // One screen-space UI for both scenes. Gameplay supplies state; all wording
    // comes from the Unity Game string tables, including dynamically formatted text.
    public sealed class GameUI : MonoBehaviour
    {
        private sealed class Label { public Text Text; public string Key; public int Size; }
        private readonly List<Label> labels = new List<Label>();
        private readonly List<Selectable> modalControls = new List<Selectable>();
        private readonly Text[] slotLabels = new Text[GameInput.SlotCount];
        private readonly Image[] slotIcons = new Image[GameInput.SlotCount];
        private readonly Button[] slotButtons = new Button[GameInput.SlotCount];
        private Font font;
        private Sprite solidSprite;
        private RectTransform safeRoot;
        private Rect safeArea;
        private Character player;
        private PlayerInteractor interactor;
        private InventoryController inventory;
        private ExamProgression exams;
        private Prepod teacher;
        private bool isMenu;
        private GameObject hud, shade, menuPage, pausePage, settingsPage, controlsPage, resultPage, loadingPage;
        private Text stats, objective, boss, prompt, feedback, itemHint, resultTitle, resultReason;
        private Image bossFill;
        private Button firstMenu, firstPause, firstResult, firstSettings, firstControls;
        private readonly List<Button> languageButtons = new List<Button>();
        private Text fullscreenValue;
        private string page = "";
        private PlayerInteractionTarget previousTarget;
        private string previousPrompt;
        private int hoveredSlot = -1;
        private bool feedbackVisible;
        private bool previousFullscreen;
        private bool previousAvailable;
        private static readonly Color Ink = new Color(0.055f, 0.08f, 0.12f, 0.98f);
        private static readonly Color Surface = new Color(0.11f, 0.16f, 0.2f, 0.98f);
        private static readonly Color Accent = new Color(0.48f, 0.88f, 0.66f, 1f);

        private void Awake()
        {
            isMenu = SceneManager.GetActiveScene().path == "Assets/menu.unity";
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            solidSprite = Sprite.Create(Texture2D.whiteTexture, new UnityEngine.Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), new Vector2(0.5f, 0.5f));
            HideLegacyUI();
            EnsureEventSystem();
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            safeRoot = Rect("Safe area", transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            UpdateSafeArea();
            if (!isMenu) BuildHud();
            BuildModals();
            GameLocalization.Changed += RefreshLanguage;
            GamePreferences.Changed += RefreshLanguage;
            GameSession.Changed += RefreshSession;
            GameplayFeedback.Changed += RefreshFeedback;
            RefreshLanguage();
            RefreshSession();
        }

        private void Start()
        {
            if (isMenu) return;
            player = FindFirstObjectByType<Character>();
            inventory = FindFirstObjectByType<InventoryController>();
            interactor = player != null ? player.GetComponent<PlayerInteractor>() : null;
            exams = ExamProgression.Instance;
            if (player != null) player.StatsChanged += RefreshStats;
            if (inventory != null) inventory.Changed += RefreshInventory;
            if (exams != null) exams.Changed += BindTeacher;
            BindTeacher();
            RefreshStats();
            RefreshInventory();
            RefreshObjective();
        }

        private void HideLegacyUI()
        {
            if (isMenu)
            {
                // The original menu uses baked button lettering; the localized
                // menu replaces that canvas while preserving the scene assets.
                foreach (Canvas old in FindObjectsByType<Canvas>(FindObjectsSortMode.None)) old.gameObject.SetActive(false);
                return;
            }
            foreach (string tag in new[] { "HealthBarSleep", "HealthBarIntel", "ResultText", "DoorStatusText" })
            {
                GameObject old = GameObject.FindGameObjectWithTag(tag);
                if (old != null && old.TryGetComponent(out Text text)) text.enabled = false;
            }
            foreach (GameObject slot in GameObject.FindGameObjectsWithTag("InventoryItems"))
                foreach (Renderer renderer in slot.GetComponentsInChildren<Renderer>()) renderer.enabled = false;
            GameObject toolbar = GameObject.FindGameObjectWithTag("ToolbarImage");
            if (toolbar != null && toolbar.TryGetComponent(out SpriteRenderer sprite)) sprite.enabled = false;
        }

        private static void EnsureEventSystem()
        {
            EventSystem system = FindFirstObjectByType<EventSystem>();
            if (system == null) system = new GameObject("EventSystem").AddComponent<EventSystem>();
            UIInputModuleBootstrap.EnsureInputModule(system);
        }

        private void BuildHud()
        {
            hud = Rect("HUD", safeRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            RectTransform top = Box("Status", hud.transform, new Vector2(0.5f, 1), new Vector2(0, -68), new Vector2(1320, 116), Ink);
            stats = TextAt(top, "", new Vector2(-370, 27), new Vector2(530, 42), 26);
            objective = TextAt(top, "", new Vector2(-280, -22), new Vector2(710, 52), 21);
            boss = TextAt(top, "", new Vector2(405, 28), new Vector2(440, 36), 21);
            RectTransform track = Box("Exam progress", top, new Vector2(0.5f, 0.5f), new Vector2(400, -17), new Vector2(420, 12), Surface);
            bossFill = Box("Remaining labs", track, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420, 12), Accent).GetComponent<Image>();
            bossFill.type = Image.Type.Filled;
            bossFill.fillMethod = Image.FillMethod.Horizontal;
            bossFill.fillOrigin = 0;
            ButtonAt(hud.transform, "menu.settings", new Vector2(0.5f, 1), new Vector2(560, -150), new Vector2(190, 42), () => { GameSession.SetPaused(true); ShowPage("settings"); }, 19);
            prompt = TextAt(hud.transform, "", new Vector2(0, -210), new Vector2(1120, 55), 25);
            feedback = TextAt(hud.transform, "", new Vector2(0, 170), new Vector2(1100, 70), 24);
            feedback.color = Accent;
            itemHint = TextAt(hud.transform, "", new Vector2(0, 0), new Vector2(1200, 54), 20);
            Anchor(itemHint.rectTransform, new Vector2(0.5f, 0), new Vector2(0, 157));
            RectTransform toolbar = Box("Inventory", hud.transform, new Vector2(0.5f, 0), new Vector2(0, 78), new Vector2(1320, 94), Ink);
            for (int i = 0; i < GameInput.SlotCount; i++)
            {
                int index = i;
                float x = -576 + i * 144;
                Button button = ButtonAt(toolbar, null, new Vector2(0.5f, 0.5f), new Vector2(x, 0), new Vector2(136, 80), () => inventory?.UseSlot(index), 17);
                slotButtons[i] = button;
                slotLabels[i] = button.GetComponentInChildren<Text>();
                slotLabels[i].rectTransform.offsetMin = new Vector2(38, 3);
                slotLabels[i].rectTransform.offsetMax = new Vector2(-3, -3);
                Image icon = Box("Item", button.transform, new Vector2(0, 0.5f), new Vector2(21, 0), new Vector2(30, 30), Color.white).GetComponent<Image>();
                icon.preserveAspect = true;
                icon.raycastTarget = false;
                slotIcons[i] = icon;
                button.gameObject.AddComponent<ItemSlotHint>().Hover = active => { hoveredSlot = active ? index : -1; RefreshHint(); };
            }
            Text cost = TextAt(hud.transform, "hud.fireCost", Vector2.zero, new Vector2(1200, 30), 18);
            Anchor(cost.rectTransform, new Vector2(0.5f, 0), new Vector2(0, 18));
        }

        private void BuildModals()
        {
            shade = Rect("Modal shade", safeRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            shade.AddComponent<Image>().color = new Color(0.025f, 0.04f, 0.065f, 0.94f);
            menuPage = Page("Main menu");
            TextAt(menuPage.transform, "game.title", new Vector2(0, 225), new Vector2(850, 85), 46).color = Accent;
            TextAt(menuPage.transform, "game.tagline", new Vector2(0, 145), new Vector2(850, 65), 26);
            firstMenu = ButtonAt(menuPage.transform, "menu.play", Center, new Vector2(0, 40), new Vector2(440, 62), () => GameSession.Load(1));
            ButtonAt(menuPage.transform, "menu.controls", Center, new Vector2(0, -40), new Vector2(440, 62), () => ShowPage("controls"));
            ButtonAt(menuPage.transform, "menu.settings", Center, new Vector2(0, -120), new Vector2(440, 62), () => ShowPage("settings"));
            ButtonAt(menuPage.transform, "menu.quit", Center, new Vector2(0, -200), new Vector2(440, 62), Application.Quit);

            pausePage = Page("Pause");
            TextAt(pausePage.transform, "menu.paused", new Vector2(0, 235), new Vector2(850, 80), 40);
            firstPause = ButtonAt(pausePage.transform, "menu.resume", Center, new Vector2(0, 110), new Vector2(440, 62), () => GameSession.SetPaused(false));
            ButtonAt(pausePage.transform, "menu.settings", Center, new Vector2(0, 25), new Vector2(440, 62), () => ShowPage("settings"));
            ButtonAt(pausePage.transform, "menu.controls", Center, new Vector2(0, -60), new Vector2(440, 62), () => ShowPage("controls"));
            ButtonAt(pausePage.transform, "menu.restart", Center, new Vector2(0, -145), new Vector2(440, 62), GameSession.Restart);
            ButtonAt(pausePage.transform, "menu.main", Center, new Vector2(0, -230), new Vector2(440, 62), GameSession.BackToMenu);

            settingsPage = Page("Settings");
            TextAt(settingsPage.transform, "menu.settings", new Vector2(0, 300), new Vector2(850, 60), 36);
            TextAt(settingsPage.transform, "settings.language", new Vector2(0, 233), new Vector2(850, 42), 24);
            for (int i = 0; i < GameLocalization.Codes.Length; i++)
            {
                int index = i;
                float x = i < 4 ? -330 + i * 220 : -220 + (i - 4) * 220;
                Button button = ButtonAt(settingsPage.transform, null, Center, new Vector2(x, i < 4 ? 178 : 116), new Vector2(207, 52), () => GameLocalization.Select(GameLocalization.Codes[index]), 21);
                button.GetComponentInChildren<Text>().text = GameLocalization.NativeNames[i];
                languageButtons.Add(button);
                if (i == 0) firstSettings = button;
            }
            TextAt(settingsPage.transform, "settings.volume", new Vector2(-245, 36), new Vector2(380, 44), 23);
            SliderAt(settingsPage.transform, new Vector2(190, 36), 0, 1, GamePreferences.Volume, GamePreferences.SetVolume);
            TextAt(settingsPage.transform, "settings.uiScale", new Vector2(-245, -39), new Vector2(380, 44), 23);
            SliderAt(settingsPage.transform, new Vector2(190, -39), 0.85f, 1.15f, GamePreferences.TextScale, GamePreferences.SetTextScale);
            TextAt(settingsPage.transform, "settings.fullscreen", new Vector2(-245, -114), new Vector2(380, 44), 23);
            Button screenButton = ButtonAt(settingsPage.transform, null, Center, new Vector2(190, -114), new Vector2(340, 48), GamePreferences.ToggleFullscreen, 23);
            fullscreenValue = screenButton.GetComponentInChildren<Text>();
            TextAt(settingsPage.transform, "settings.languageNote", new Vector2(0, -207), new Vector2(870, 95), 19);
            ButtonAt(settingsPage.transform, "menu.back", Center, new Vector2(0, -300), new Vector2(340, 54), Back);

            controlsPage = Page("How to play");
            TextAt(controlsPage.transform, "menu.controls", new Vector2(0, 300), new Vector2(900, 60), 36);
            Text controls = TextAt(controlsPage.transform, "controls.body", new Vector2(0, 10), new Vector2(870, 485), 22);
            controls.alignment = TextAnchor.UpperLeft;
            firstControls = ButtonAt(controlsPage.transform, "menu.back", Center, new Vector2(0, -300), new Vector2(340, 54), Back);

            resultPage = Page("Result");
            resultTitle = TextAt(resultPage.transform, "", new Vector2(0, 200), new Vector2(850, 85), 42);
            resultTitle.color = Accent;
            resultReason = TextAt(resultPage.transform, "", new Vector2(0, 80), new Vector2(850, 120), 25);
            firstResult = ButtonAt(resultPage.transform, "menu.restart", Center, new Vector2(0, -65), new Vector2(440, 62), GameSession.Restart);
            ButtonAt(resultPage.transform, "menu.main", Center, new Vector2(0, -150), new Vector2(440, 62), GameSession.BackToMenu);
            TextAt(resultPage.transform, "result.hint", new Vector2(0, -250), new Vector2(850, 54), 21);
            loadingPage = Page("Loading");
            TextAt(loadingPage.transform, "menu.loading", Vector2.zero, new Vector2(850, 100), 40);
        }

        private static Vector2 Center { get { return new Vector2(0.5f, 0.5f); } }
        private GameObject Page(string name) { return Box(name, shade.transform, Center, Vector2.zero, new Vector2(1000, 720), Ink).gameObject; }
        private void Back() { GamePreferences.Save(); ShowPage(isMenu ? "menu" : "pause"); }
        private void ShowPage(string next)
        {
            page = next;
            shade.SetActive(next.Length != 0);
            menuPage.SetActive(next == "menu");
            pausePage.SetActive(next == "pause");
            settingsPage.SetActive(next == "settings");
            controlsPage.SetActive(next == "controls");
            resultPage.SetActive(next == "result");
            loadingPage.SetActive(next == "loading");
            Button selection = next == "menu" ? firstMenu : next == "pause" ? firstPause : next == "settings" ? firstSettings : next == "controls" ? firstControls : next == "result" ? firstResult : null;
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(selection != null ? selection.gameObject : null);
        }
        private void RefreshSession()
        {
            RefreshObjective();
            if (GameSession.IsLoading) ShowPage("loading");
            else if (GameSession.IsOver) ShowPage("result");
            else if (isMenu) { if (page.Length == 0) ShowPage("menu"); }
            else if (GameSession.IsPaused) { if (page.Length == 0) ShowPage("pause"); }
            else ShowPage("");
            RefreshResult();
        }
        private void RefreshLanguage()
        {
            foreach (Label label in labels)
            {
                label.Text.fontSize = Mathf.RoundToInt(label.Size * GamePreferences.TextScale);
                label.Text.resizeTextMaxSize = label.Text.fontSize;
                if (!string.IsNullOrEmpty(label.Key)) label.Text.text = GameLocalization.Get(label.Key);
            }
            foreach (Button button in languageButtons) button.interactable = GameLocalization.IsReady;
            previousAvailable = GameLocalization.IsReady && !GameSession.IsLoading;
            foreach (Selectable control in modalControls) control.interactable = previousAvailable;
            for (int i = 0; i < languageButtons.Count; i++)
                languageButtons[i].GetComponent<Image>().color = GameLocalization.Codes[i] == GameLocalization.LanguageCode ? new Color(0.17f, 0.36f, 0.27f) : Surface;
            RefreshStats(); RefreshInventory(); RefreshObjective(); RefreshBoss(); RefreshResult(); RefreshPrompt(); RefreshHint(); RefreshFeedback();
            RefreshFullscreen();
        }
        private void RefreshStats() { if (stats != null && player != null) stats.text = GameLocalization.Get("hud.stats", player.Sleep, player.Intelligence); }
        private void RefreshObjective()
        {
            if (objective == null) return;
            string key = exams != null && exams.Stage == 2 ? "objective.finalExam" : GameSession.PickupsCollected == 0 ? "objective.prepare" : "objective.firstExam";
            objective.text = GameLocalization.Get(key);
        }
        private void BindTeacher()
        {
            if (teacher != null) teacher.HealthChanged -= RefreshBoss;
            teacher = exams != null ? exams.Current : null;
            if (teacher != null) teacher.HealthChanged += RefreshBoss;
            RefreshBoss(); RefreshObjective();
        }
        private void RefreshBoss()
        {
            if (boss == null) return;
            boss.text = teacher != null ? GameLocalization.Get("hud.boss", exams != null ? exams.Stage : 1, teacher.RemainingLabs, teacher.RequiredLabs) : GameLocalization.Get("hud.nextExam");
            bossFill.fillAmount = teacher != null ? (float)teacher.RemainingLabs / teacher.RequiredLabs : 0;
        }
        private void RefreshInventory()
        {
            if (inventory == null) return;
            for (int i = 0; i < slotLabels.Length; i++)
            {
                InventoryItem item = inventory.ItemAt(i);
                slotLabels[i].text = item != null ? GameLocalization.Get("hud.slot", i + 1, GameLocalization.Get(item.NameKey), item.Count) : (i + 1) + "\n" + GameLocalization.Get("hud.empty");
                slotIcons[i].enabled = item != null && item.sprite != null;
                if (slotIcons[i].enabled) slotIcons[i].sprite = item.sprite.sprite;
                slotButtons[i].interactable = item != null;
            }
            RefreshHint();
        }
        private void RefreshPrompt()
        {
            if (prompt == null) return;
            PlayerInteractionTarget target = interactor != null ? interactor.Target : null;
            prompt.text = target != null ? GameLocalization.Get(target.PromptKey, GameLocalization.Get(target.ItemKey)) : string.Empty;
            previousTarget = target;
            previousPrompt = target != null ? target.PromptKey : null;
        }
        private void RefreshHint()
        {
            if (itemHint == null) return;
            InventoryItem item = inventory != null ? inventory.ItemAt(hoveredSlot) : null;
            itemHint.text = item != null ? GameLocalization.Get(item.EffectKey) : string.Empty;
        }
        private void RefreshFeedback()
        {
            if (feedback == null) return;
            feedbackVisible = Time.unscaledTime < GameplayFeedback.ExpiresAt;
            feedback.text = feedbackVisible ? GameLocalization.Get(GameplayFeedback.Key, GameLocalization.Get(GameplayFeedback.DetailKey)) : string.Empty;
        }
        private void RefreshResult()
        {
            if (resultTitle == null) return;
            resultTitle.text = GameLocalization.Get(GameSession.Result == SessionResult.Won ? "result.won" : "result.lost");
            resultReason.text = GameSession.Result == SessionResult.Won ? GameLocalization.Get("game.tagline") : GameLocalization.Get(GameSession.LossReasonKey);
        }
        private void Update()
        {
            if (safeArea != Screen.safeArea) UpdateSafeArea();
            if (!GameSession.IsOver && !GameSession.IsLoading && GameInput.PausePressed)
            {
                if (page == "settings" || page == "controls") Back();
                else if (!isMenu) GameSession.SetPaused(!GameSession.IsPaused);
            }
            if (Screen.fullScreen != previousFullscreen) RefreshFullscreen();
            if (feedbackVisible && Time.unscaledTime >= GameplayFeedback.ExpiresAt) RefreshFeedback();
            PlayerInteractionTarget target = interactor != null ? interactor.Target : null;
            if (target != previousTarget || (target != null && target.PromptKey != previousPrompt)) RefreshPrompt();
            bool available = GameLocalization.IsReady && !GameSession.IsLoading;
            if (available != previousAvailable)
            {
                previousAvailable = available;
                foreach (Selectable control in modalControls) control.interactable = available;
            }
        }
        private void RefreshFullscreen()
        {
            previousFullscreen = Screen.fullScreen;
            if (fullscreenValue != null) fullscreenValue.text = GameLocalization.Get(previousFullscreen ? "settings.on" : "settings.off");
        }
        private void UpdateSafeArea()
        {
            safeArea = Screen.safeArea;
            if (Screen.width == 0 || Screen.height == 0) return;
            safeRoot.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
            safeRoot.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
        }
        private void OnDestroy()
        {
            GameLocalization.Changed -= RefreshLanguage;
            GamePreferences.Changed -= RefreshLanguage;
            GameSession.Changed -= RefreshSession;
            GameplayFeedback.Changed -= RefreshFeedback;
            if (player != null) player.StatsChanged -= RefreshStats;
            if (inventory != null) inventory.Changed -= RefreshInventory;
            if (exams != null) exams.Changed -= BindTeacher;
            if (teacher != null) teacher.HealthChanged -= RefreshBoss;
            GamePreferences.Save();
            if (solidSprite != null) Destroy(solidSprite);
        }

        private RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max, Vector2 position, Vector2 size)
        {
            RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.anchorMin = min; rect.anchorMax = max;
            rect.anchoredPosition = position; rect.sizeDelta = size;
            return rect;
        }
        private static void Anchor(RectTransform rect, Vector2 anchor, Vector2 position) { rect.anchorMin = rect.anchorMax = anchor; rect.anchoredPosition = position; }
        private RectTransform Box(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, Color color)
        {
            RectTransform rect = Rect(name, parent, anchor, anchor, position, size);
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = solidSprite;
            image.color = color;
            return rect;
        }
        private Text TextAt(Transform parent, string key, Vector2 position, Vector2 size, int fontSize)
        {
            RectTransform rect = Rect(string.IsNullOrEmpty(key) ? "Text" : key, parent, Center, Center, position, size);
            Text text = rect.gameObject.AddComponent<Text>();
            text.font = font; text.fontSize = fontSize; text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter; text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true; text.resizeTextMinSize = 14; text.resizeTextMaxSize = fontSize;
            labels.Add(new Label { Text = text, Key = key, Size = fontSize });
            return text;
        }
        private Button ButtonAt(Transform parent, string key, Vector2 anchor, Vector2 position, Vector2 size, Action action, int fontSize = 25)
        {
            RectTransform rect = Box(key ?? "Button", parent, anchor, position, size, Surface);
            Button button = rect.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors; colors.highlightedColor = new Color(0.62f, 0.92f, 0.76f); colors.selectedColor = colors.highlightedColor;
            button.colors = colors; button.targetGraphic = rect.GetComponent<Image>();
            button.onClick.AddListener(() => action());
            Text text = TextAt(rect, key, Vector2.zero, size - new Vector2(20, 8), fontSize);
            text.rectTransform.anchorMin = Vector2.zero; text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(10, 4); text.rectTransform.offsetMax = new Vector2(-10, -4);
            if (shade != null && rect.IsChildOf(shade.transform)) modalControls.Add(button);
            return button;
        }
        private Slider SliderAt(Transform parent, Vector2 position, float min, float max, float value, Action<float> action)
        {
            RectTransform rect = Box("Slider", parent, Center, position, new Vector2(340, 42), Surface);
            Slider slider = rect.gameObject.AddComponent<Slider>();
            Image fill = Box("Fill", rect, Center, Vector2.zero, new Vector2(300, 8), Accent).GetComponent<Image>();
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.rectTransform.sizeDelta = new Vector2(-24, -34);
            RectTransform handle = Box("Handle", rect, Center, Vector2.zero, new Vector2(22, -12), Color.white);
            slider.fillRect = fill.rectTransform; slider.handleRect = handle;
            slider.targetGraphic = handle.GetComponent<Image>(); slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = min; slider.maxValue = max; slider.SetValueWithoutNotify(value);
            slider.onValueChanged.AddListener(v => action(v));
            modalControls.Add(slider);
            return slider;
        }
    }
}
