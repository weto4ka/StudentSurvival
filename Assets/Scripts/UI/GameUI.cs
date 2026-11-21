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
    public sealed partial class GameUI : MonoBehaviour
    {
        private sealed class Label { public Text Text; public string Key; public int Size; }
        private sealed class BindingRow { public GameInput.BindingOption Option; public Text Name, Value; }
        private readonly List<Label> labels = new List<Label>();
        private readonly List<Selectable> modalControls = new List<Selectable>();
        private readonly List<BindingRow> bindingRows = new List<BindingRow>();
        private readonly Stack<string> history = new Stack<string>();
        private readonly Text[] slotLabels = new Text[GameInput.SlotCount];
        private readonly Image[] slotIcons = new Image[GameInput.SlotCount];
        private readonly Button[] slotButtons = new Button[GameInput.SlotCount];
        private Font font;
        private Sprite solid;
        private RectTransform safeRoot, bindingContent;
        private Rect safeArea;
        private Character player;
        private PlayerInteractor interactor;
        private InventoryController inventory;
        private ExamProgression exams;
        private Prepod teacher;
        private bool isMenu, bindingsGamepad, inventoryPinned, inventoryVisible, dropdownWasExpanded;
        private GameObject hud, shade, menuPage, pausePage, settingsPage, controlsPage, resultPage, loadingPage, bossRoot;
        private Text stats, objective, boss, prompt, feedback, itemHint, resultTitle, resultReason, fullscreenValue, rebindStatus;
        private Image bossFill;
        private CanvasGroup inventoryGroup, objectiveGroup;
        private Button firstMenu, firstPause, firstResult, firstControls, newRunButton, gear, fullscreenButton, cancelRebind, keyboardTab, controllerTab;
        private Dropdown languages;
        private DropdownOpenState languageState;
        private ScrollRect bindingScroll;
        private string page = "", objectiveKey;
        private PlayerInteractionTarget previousTarget;
        private string previousPrompt;
        private int hoveredSlot = -1;
        private int lastNavigationFrame = -1;
        private float inventoryUntil, objectiveUntil, bossUntil, tutorialUntil;
        private bool previousAvailable;
        private static readonly Color Glass = new Color(0.035f, 0.06f, 0.08f, 0.23f);
        private static readonly Color Surface = new Color(0.08f, 0.15f, 0.18f, 0.5f);
        private static readonly Color Accent = new Color(0.56f, 0.94f, 0.74f, 1);
        private static Vector2 Center { get { return new Vector2(0.5f, 0.5f); } }

        private void Awake()
        {
            isMenu = SceneManager.GetActiveScene().path == "Assets/menu.unity";
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            solid = Sprite.Create(Texture2D.whiteTexture, new UnityEngine.Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), Center);
            HideLegacyUI();
            EventSystem system = FindFirstObjectByType<EventSystem>();
            if (system == null) system = new GameObject("EventSystem").AddComponent<EventSystem>();
            UIInputModuleBootstrap.EnsureInputModule(system);
            Canvas canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 200;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            safeRoot = Rect("Safe area", transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            UpdateSafeArea();
            if (!isMenu) BuildHud();
            BuildModals();
            BuildRunFlow();
            gear = IconButton(safeRoot, new Vector2(1, 1), new Vector2(-34, -32), true, OpenSettings);
            GameLocalization.Changed += RefreshLanguage; GamePreferences.Changed += RefreshLanguage;
            RunSaveService.Changed += RefreshSave;
            GameSession.Changed += RefreshSession; GameplayFeedback.Changed += RefreshFeedback; GameInput.Changed += OnInputChanged;
            RefreshLanguage(); RefreshSession();
        }
        private void Start()
        {
            if (isMenu) return;
            player = FindFirstObjectByType<Character>(); inventory = FindFirstObjectByType<InventoryController>();
            interactor = player != null ? player.GetComponent<PlayerInteractor>() : null; exams = ExamProgression.Instance;
            if (player != null) player.StatsChanged += RefreshStats;
            if (inventory != null) inventory.Changed += InventoryChanged;
            if (exams != null) exams.Changed += BindTeacher;
            tutorialUntil = Time.unscaledTime + 7;
            BindTeacher(); RefreshStats(); RefreshInventory(); RefreshObjective(); RefreshHint();
            route = gameObject.AddComponent<ObjectiveRoute>();
            if (GameSession.TakeFirstBriefing()) { GameSession.SetPaused(true); history.Push(""); ShowPage("briefing"); }
        }
        private void HideLegacyUI()
        {
            if (isMenu)
            {
                foreach (Canvas old in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
                {
                    old.gameObject.name = "Original menu artwork";
                    foreach (Graphic graphic in old.GetComponentsInChildren<Graphic>(true))
                    { graphic.enabled = graphic.gameObject.name == "menu"; graphic.raycastTarget = false; }
                    foreach (Selectable control in old.GetComponentsInChildren<Selectable>(true)) control.gameObject.SetActive(false);
                    GraphicRaycaster raycaster = old.GetComponent<GraphicRaycaster>(); if (raycaster != null) raycaster.enabled = false;
                }
                return;
            }
            foreach (string tag in new[] { "HealthBarSleep", "HealthBarIntel", "ResultText", "DoorStatusText", "HealthBar" })
            {
                GameObject old = GameObject.FindGameObjectWithTag(tag);
                if (old != null && old.TryGetComponent(out Graphic graphic)) graphic.enabled = false;
            }
            foreach (GameObject slot in GameObject.FindGameObjectsWithTag("InventoryItems"))
                foreach (Renderer renderer in slot.GetComponentsInChildren<Renderer>()) renderer.enabled = false;
            GameObject toolbar = GameObject.FindGameObjectWithTag("ToolbarImage");
            if (toolbar != null && toolbar.TryGetComponent(out SpriteRenderer sprite)) sprite.enabled = false;
        }
        private void BuildHud()
        {
            hud = Rect("HUD", safeRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            RectTransform statPanel = Box("Stats", hud.transform, new Vector2(0, 1), new Vector2(205, -29), new Vector2(370, 44), Glass);
            statPanel.GetComponent<Image>().raycastTarget = false;
            stats = TextAt(statPanel, "", Vector2.zero, new Vector2(350, 40), 23); stats.alignment = TextAnchor.MiddleLeft;
            objective = TextAt(hud.transform, "", Vector2.zero, new Vector2(950, 40), 22);
            Anchor(objective.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -76)); objectiveGroup = objective.gameObject.AddComponent<CanvasGroup>();
            bossRoot = Box("Exam", hud.transform, new Vector2(0.5f, 1), new Vector2(0, -28), new Vector2(400, 44), Glass).gameObject;
            bossRoot.GetComponent<Image>().raycastTarget = false;
            boss = TextAt(bossRoot.transform, "", new Vector2(0, 4), new Vector2(390, 30), 19);
            RectTransform track = Box("Progress", bossRoot.transform, Center, new Vector2(0, -17), new Vector2(360, 4), Surface); track.GetComponent<Image>().raycastTarget = false;
            bossFill = Box("Remaining", track, Center, Vector2.zero, new Vector2(360, 4), Accent).GetComponent<Image>();
            bossFill.raycastTarget = false; bossFill.type = Image.Type.Filled; bossFill.fillMethod = Image.FillMethod.Horizontal; bossFill.fillOrigin = 0;
            prompt = TextAt(hud.transform, "", Vector2.zero, new Vector2(1100, 42), 23); Anchor(prompt.rectTransform, new Vector2(0.5f, 0), new Vector2(0, 133));
            feedback = TextAt(hud.transform, "", Vector2.zero, new Vector2(1050, 45), 22); feedback.color = Accent; Anchor(feedback.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -122));
            itemHint = TextAt(hud.transform, "", Vector2.zero, new Vector2(1150, 52), 19); Anchor(itemHint.rectTransform, new Vector2(0.5f, 0), new Vector2(0, 86));
            RectTransform toolbar = Rect("Inventory", hud.transform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 32), new Vector2(580, 58));
            inventoryGroup = toolbar.gameObject.AddComponent<CanvasGroup>();
            for (int i = 0; i < GameInput.SlotCount; i++)
            {
                int index = i;
                Button button = ButtonAt(toolbar, null, Center, Vector2.zero, new Vector2(58, 54), () => inventory?.UseSlot(index), 15);
                slotButtons[i] = button; slotLabels[i] = button.GetComponentInChildren<Text>();
                Anchor(slotLabels[i].rectTransform, new Vector2(0.5f, 0), new Vector2(0, 10)); slotLabels[i].rectTransform.sizeDelta = new Vector2(58, 22);
                Image icon = Box("Item", button.transform, Center, new Vector2(0, 8), new Vector2(26, 26), Color.white).GetComponent<Image>();
                icon.preserveAspect = true; icon.raycastTarget = false; slotIcons[i] = icon;
                button.gameObject.AddComponent<ItemSlotHint>().Hover = active => { hoveredSlot = active ? index : -1; RefreshHint(); };
            }
            inventoryGroup.alpha = 0; inventoryGroup.blocksRaycasts = false;
        }
        private void BuildModals()
        {
            shade = Rect("Modal shade", safeRoot, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            shade.AddComponent<Image>().color = new Color(0.01f, 0.025f, 0.045f, 0.3f);
            menuPage = Page("Main menu", false);
            TextAt(menuPage.transform, "game.title", new Vector2(0, 180), new Vector2(900, 85), 46).color = Accent;
            TextAt(menuPage.transform, "game.tagline", new Vector2(0, 106), new Vector2(850, 52), 25);
            firstMenu = ButtonAt(menuPage.transform, "save.continue", Center, new Vector2(0, 35), new Vector2(370, 56), GameSession.ContinueRun);
            newRunButton = ButtonAt(menuPage.transform, "save.newRun", Center, new Vector2(0, -30), new Vector2(370, 52), ConfirmNewRun);
            ButtonAt(menuPage.transform, "menu.controls", Center, new Vector2(0, -95), new Vector2(370, 52), OpenBriefing);
            ButtonAt(menuPage.transform, "menu.quit", Center, new Vector2(0, -225), new Vector2(370, 52), QuitGame);
            pausePage = Page("Pause", true);
            TextAt(pausePage.transform, "pause.title", new Vector2(0, 180), new Vector2(700, 60), 34);
            firstPause = ButtonAt(pausePage.transform, "menu.resume", Center, new Vector2(0, 65), new Vector2(340, 50), () => GameSession.SetPaused(false));
            ButtonAt(pausePage.transform, "save.retry", Center, new Vector2(0, 0), new Vector2(340, 50), ConfirmRetry);
            ButtonAt(pausePage.transform, "menu.controls", Center, new Vector2(0, -65), new Vector2(340, 50), OpenBriefing);
            ButtonAt(pausePage.transform, "menu.main", Center, new Vector2(0, -130), new Vector2(340, 50), GameSession.BackToMenu);
            settingsPage = Page("Settings", true);
            TextAt(settingsPage.transform, "menu.settings", new Vector2(0, 265), new Vector2(800, 60), 34);
            SettingLabel("settings.language", 165);
            languages = DropdownAt(settingsPage.transform, new Vector2(200, 165)); languages.AddOptions(new List<string>(GameLocalization.NativeNames));
            languages.onValueChanged.AddListener(index => { UIAudio.Click(); GameLocalization.Select(GameLocalization.Codes[index]); });
            SettingLabel("settings.volume", 90); SliderAt(settingsPage.transform, new Vector2(200, 90), 0, 1, GamePreferences.Volume, GamePreferences.SetVolume);
            SettingLabel("settings.uiVolume", 15); SliderAt(settingsPage.transform, new Vector2(200, 15), 0, 1, GamePreferences.UiVolume, GamePreferences.SetUiVolume);
            SettingLabel("settings.uiScale", -60); SliderAt(settingsPage.transform, new Vector2(200, -60), 0.85f, 1.15f, GamePreferences.TextScale, GamePreferences.SetTextScale);
            SettingLabel("settings.windowMode", -135);
            fullscreenButton = ButtonAt(settingsPage.transform, null, Center, new Vector2(200, -135), new Vector2(360, 48), GamePreferences.ToggleFullscreen, 21);
            fullscreenValue = fullscreenButton.GetComponentInChildren<Text>();
            ButtonAt(settingsPage.transform, "menu.controls", Center, new Vector2(0, -285), new Vector2(360, 50), OpenControls, 23);
            controlsPage = Page("Controls", true);
            TextAt(controlsPage.transform, "menu.controls", new Vector2(0, 282), new Vector2(800, 55), 32);
            TextAt(controlsPage.transform, "controls.goal", new Vector2(0, 210), new Vector2(870, 88), 20);
            keyboardTab = ButtonAt(controlsPage.transform, "controls.keyboard", Center, new Vector2(-200, 133), new Vector2(350, 44), () => BuildBindings(false), 22);
            controllerTab = ButtonAt(controlsPage.transform, "controls.gamepad", Center, new Vector2(200, 133), new Vector2(350, 44), () => BuildBindings(true), 22); firstControls = keyboardTab;
            GameObject scroll = DefaultControls.CreateScrollView(StandardResources()); scroll.name = "Editable bindings"; scroll.transform.SetParent(controlsPage.transform, false);
            RectTransform scrollRect = scroll.GetComponent<RectTransform>(); Anchor(scrollRect, Center, new Vector2(0, -54)); scrollRect.sizeDelta = new Vector2(880, 310);
            scroll.GetComponent<Image>().color = new Color(0, 0, 0, 0.08f);
            bindingScroll = scroll.GetComponent<ScrollRect>(); bindingScroll.horizontal = false; bindingScroll.scrollSensitivity = 35; bindingContent = bindingScroll.content;
            foreach (Scrollbar scrollbar in scroll.GetComponentsInChildren<Scrollbar>())
            {
                scrollbar.GetComponent<Image>().color = Glass;
                if (scrollbar.targetGraphic != null) scrollbar.targetGraphic.color = new Color(0.6f, 0.8f, 0.7f, 0.5f);
                scrollbar.gameObject.AddComponent<UIControlSound>().Cancel = HandleCancel;
            }
            rebindStatus = TextAt(controlsPage.transform, "", new Vector2(0, -237), new Vector2(850, 42), 18);
            ButtonAt(controlsPage.transform, "controls.reset", Center, new Vector2(-200, -292), new Vector2(350, 44), GameInput.RestoreDefaults, 21);
            cancelRebind = ButtonAt(controlsPage.transform, "controls.cancel", Center, new Vector2(200, -292), new Vector2(350, 44), GameInput.CancelRebind, 21);
            BuildBindings(false);
            resultPage = Page("Result", true);
            resultTitle = TextAt(resultPage.transform, "", new Vector2(0, 165), new Vector2(850, 80), 40); resultTitle.color = Accent;
            resultReason = TextAt(resultPage.transform, "", new Vector2(0, 52), new Vector2(850, 110), 24);
            firstResult = ButtonAt(resultPage.transform, "save.retry", Center, new Vector2(0, -70), new Vector2(370, 52), GameSession.Restart);
            ButtonAt(resultPage.transform, "menu.main", Center, new Vector2(0, -140), new Vector2(370, 52), GameSession.BackToMenu);
            loadingPage = Page("Loading", false); TextAt(loadingPage.transform, "menu.loading", Vector2.zero, new Vector2(850, 90), 36);
        }
        private void SettingLabel(string key, float y)
        {
            Text label = TextAt(settingsPage.transform, key, new Vector2(-235, y), new Vector2(340, 48), 22); label.alignment = TextAnchor.MiddleLeft;
        }
        private GameObject Page(string name, bool back)
        {
            GameObject result = Box(name, shade.transform, Center, Vector2.zero, new Vector2(960, 690), new Color(0.02f, 0.04f, 0.06f, 0.12f)).gameObject;
            if (back) IconButton(result.transform, new Vector2(0, 1), new Vector2(35, -35), false, Back);
            return result;
        }
        private void OpenSettings()
        {
            if (page == "settings") { Back(); return; }
            string from = page;
            if (!isMenu && !GameSession.IsOver) GameSession.SetPaused(true);
            history.Push(from); ShowPage("settings");
        }
        private void OpenControls() { history.Push(page); BuildBindings(GameInput.UsingGamepad); ShowPage("controls"); }
        private void Back()
        {
            lastNavigationFrame = Time.frameCount;
            if (GameInput.IsRebinding) { GameInput.CancelRebind(); return; }
            GamePreferences.Save();
            if (GameSession.IsOver) { GameSession.BackToMenu(); return; }
            string next = history.Count > 0 ? history.Pop() : isMenu ? "menu" : "";
            if (next == "" && !isMenu) GameSession.SetPaused(false); else ShowPage(next);
        }
        private void HandleCancel()
        {
            if (lastNavigationFrame == Time.frameCount || GameInput.ShortcutsSuppressed || page.Length == 0 || page == "menu" || page == "loading") return;
            Back();
        }
        private void ShowPage(string next)
        {
            lastNavigationFrame = Time.frameCount;
            page = next; shade.SetActive(next.Length != 0); if (hud != null) hud.SetActive(next.Length == 0);
            menuPage.SetActive(next == "menu"); pausePage.SetActive(next == "pause"); settingsPage.SetActive(next == "settings");
            ShowRunPage(next);
            controlsPage.SetActive(next == "controls"); resultPage.SetActive(next == "result"); loadingPage.SetActive(next == "loading");
            if (EventSystem.current != null)
            {
                EventSystem.current.sendNavigationEvents = next.Length != 0;
                Selectable selection = null;
                switch (next)
                {
                    case "menu": selection = RunSaveService.HasContinue ? firstMenu : newRunButton; break;
                    case "pause": selection = firstPause; break;
                    case "settings": selection = languages; break;
                    case "controls": selection = firstControls; break;
                    case "result": selection = firstResult; break;
                    case "confirmNew": selection = cancelNew; break;
                    case "confirmRetry": selection = cancelRetry; break;
                    case "credits": selection = creditsBack; break;
                    case "briefing": selection = briefingReady; break;
                }
                EventSystem.current.SetSelectedGameObject(selection != null ? selection.gameObject : null);
            }
            ApplyAvailability();
        }
        private void RefreshSession()
        {
            RefreshObjective();
            if (GameSession.IsLoading) { history.Clear(); ShowPage("loading"); }
            else if (GameSession.IsOver) { history.Clear(); ShowPage("result"); }
            else if (isMenu) { if (page.Length == 0) ShowPage("menu"); }
            else if (GameSession.IsPaused) { if (page.Length == 0) { history.Clear(); ShowPage("pause"); } }
            else { history.Clear(); ShowPage(""); }
            RefreshResult(); RefreshRunLanguage();
        }
        private void RefreshLanguage()
        {
            foreach (Label label in labels)
            {
                if (label.Text == null) continue;
                label.Text.fontSize = Mathf.RoundToInt(label.Size * GamePreferences.TextScale); label.Text.resizeTextMaxSize = label.Text.fontSize;
                if (!string.IsNullOrEmpty(label.Key)) label.Text.text = GameLocalization.Get(label.Key);
            }
            if (languages != null) languages.SetValueWithoutNotify(Mathf.Max(0, Array.IndexOf(GameLocalization.Codes, GameLocalization.LanguageCode)));
            RefreshStats(); RefreshInventory(); RefreshObjective(); RefreshBoss(); RefreshResult(); RefreshPrompt(); RefreshHint(); RefreshFeedback(); RefreshBindings(); RefreshFullscreen(); RefreshRunLanguage(); ApplyAvailability();
        }
        private void RefreshStats() { if (stats != null && player != null) stats.text = GameLocalization.Get("hud.stats", player.Sleep, player.Intelligence); }
        private void RefreshObjective()
        {
            if (objective == null) return;
            string key = exams != null && exams.Stage == 2 ? "objective.finalExam" : GameSession.PickupsCollected == 0 ? "objective.prepareDynamic" : "objective.firstExam";
            if (objectiveKey != key) { objectiveKey = key; objectiveUntil = Time.unscaledTime + 7; }
            objective.text = GameLocalization.Get(key, GameInput.Display("Action"));
        }
        private void BindTeacher()
        {
            if (teacher != null) teacher.HealthChanged -= TeacherHit;
            teacher = exams != null ? exams.Current : null;
            if (teacher != null) teacher.HealthChanged += TeacherHit;
            RefreshBoss(); RefreshObjective();
        }
        private void TeacherHit() { bossUntil = Time.unscaledTime + 4; RefreshBoss(); }
        private void RefreshBoss()
        {
            if (boss == null) return;
            boss.text = teacher != null ? GameLocalization.Get("hud.boss", exams != null ? exams.Stage : 1, teacher.RemainingLabs, teacher.RequiredLabs) : GameLocalization.Get("hud.nextExam");
            bossFill.fillAmount = teacher != null ? (float)teacher.RemainingLabs / teacher.RequiredLabs : 0;
        }
        private void InventoryChanged() { inventoryUntil = Time.unscaledTime + 4; RefreshInventory(); }
        private void RefreshInventory()
        {
            if (inventory == null) return;
            int count = 0; for (int i = 0; i < slotButtons.Length; i++) if (inventory.ItemAt(i) != null) count++;
            int visible = 0;
            for (int i = 0; i < slotButtons.Length; i++)
            {
                InventoryItem item = inventory.ItemAt(i); slotButtons[i].gameObject.SetActive(item != null);
                if (item == null) continue;
                Anchor(slotButtons[i].GetComponent<RectTransform>(), Center, new Vector2((visible++ - (count - 1) * 0.5f) * 64, 0));
                string binding = GameInput.UsingGamepad ? (i + 1).ToString() : GameInput.SlotDisplay(i);
                slotLabels[i].text = binding + "  ×" + item.Count;
                slotIcons[i].enabled = item.sprite != null; if (item.sprite != null) slotIcons[i].sprite = item.sprite.sprite;
                slotButtons[i].GetComponent<Image>().color = inventory.SelectedSlot == i ? new Color(0.2f, 0.5f, 0.35f, 0.5f) : Glass;
            }
            RefreshHint();
        }
        private void RefreshPrompt()
        {
            if (prompt == null) return;
            PlayerInteractionTarget target = interactor != null ? interactor.Target : null;
            string key = target != null ? target.PromptKey : null;
            prompt.text = key == "pickup.prompt" ? GameLocalization.Get("hint.pickup", GameInput.Display("Action"), GameLocalization.Get(target.ItemKey))
                : key == "door.open" ? GameLocalization.Get("hint.door", GameInput.Display("Action")) : GameLocalization.Get(key);
            if (key == "flirt.prompt") prompt.text = GameLocalization.Get(key, GameInput.Display("Action"));
            previousTarget = target; previousPrompt = key;
        }
        private void RefreshHint()
        {
            if (itemHint == null) return;
            int index = hoveredSlot >= 0 ? hoveredSlot : inventory != null && inventoryVisible && GameInput.UsingGamepad ? inventory.SelectedSlot : -1;
            InventoryItem item = inventory != null ? inventory.ItemAt(index) : null;
            itemHint.text = item != null ? GameLocalization.Get(item.NameKey) + " · " + GameLocalization.Get(item.EffectKey)
                : Time.unscaledTime < tutorialUntil ? GameLocalization.Get("hint.start", GameInput.Display("Fire"), GameInput.Display("Inventory"), GameInput.Display("UseItem")) : string.Empty;
        }
        private void RefreshFeedback()
        {
            if (feedback != null) feedback.text = Time.unscaledTime < GameplayFeedback.ExpiresAt ? GameLocalization.Get(GameplayFeedback.Key, GameLocalization.Get(GameplayFeedback.DetailKey)) : string.Empty;
        }
        private void RefreshResult()
        {
            if (resultTitle == null) return;
            resultTitle.text = GameLocalization.Get(GameSession.Result == SessionResult.Won ? "result.won" : "result.lost");
            resultReason.text = GameSession.Result == SessionResult.Won ? GameLocalization.Get("game.tagline") : GameLocalization.Get(GameSession.LossReasonKey);
        }
        private void BuildBindings(bool gamepad)
        {
            if (bindingContent == null || GameInput.IsRebinding) return;
            bindingsGamepad = gamepad;
            labels.RemoveAll(label => label.Text == null || label.Text.transform.IsChildOf(bindingContent));
            modalControls.RemoveAll(control => control == null || control.transform.IsChildOf(bindingContent));
            for (int i = bindingContent.childCount - 1; i >= 0; i--) { bindingContent.GetChild(i).gameObject.SetActive(false); Destroy(bindingContent.GetChild(i).gameObject); }
            bindingRows.Clear();
            List<GameInput.BindingOption> options = GameInput.Bindings(gamepad);
            bindingContent.anchorMin = Vector2.up; bindingContent.anchorMax = Vector2.one; bindingContent.pivot = Vector2.up;
            bindingContent.sizeDelta = new Vector2(0, options.Count * 56 + 16); bindingContent.anchoredPosition = Vector2.zero;
            for (int i = 0; i < options.Count; i++)
            {
                GameInput.BindingOption option = options[i];
                RectTransform row = Rect("Binding", bindingContent, Vector2.up, Vector2.one, new Vector2(0, -36 - i * 56), new Vector2(-30, 48));
                Text name = TextAt(row, "", new Vector2(-185, 0), new Vector2(430, 44), 21); name.alignment = TextAnchor.MiddleLeft;
                Button button = ButtonAt(row, null, Center, new Vector2(218, 0), new Vector2(340, 44), () => GameInput.Rebind(option, bindingsGamepad), 20);
                button.gameObject.AddComponent<ScrollSelectionIntoView>().Scroll = bindingScroll;
                bindingRows.Add(new BindingRow { Option = option, Name = name, Value = button.GetComponentInChildren<Text>() });
            }
            RefreshBindings(); ApplyAvailability();
        }
        private void RefreshBindings()
        {
            foreach (BindingRow row in bindingRows)
            {
                row.Name.text = row.Option.SlotNumber > 0 ? GameLocalization.Get(row.Option.LabelKey, row.Option.SlotNumber) : GameLocalization.Get(row.Option.LabelKey);
                row.Value.text = row.Option.Display;
            }
            if (keyboardTab != null) keyboardTab.GetComponent<Image>().color = bindingsGamepad ? Surface : new Color(0.2f, 0.48f, 0.34f, 0.65f);
            if (controllerTab != null) controllerTab.GetComponent<Image>().color = bindingsGamepad ? new Color(0.2f, 0.48f, 0.34f, 0.65f) : Surface;
            if (rebindStatus != null) rebindStatus.text = GameLocalization.Get(GameInput.IsRebinding ? "controls.listening" : GameInput.RebindConflict ? "controls.conflict" : "controls.editHint");
        }
        private void OnInputChanged() { RefreshBindings(); RefreshInventory(); RefreshObjective(); RefreshPrompt(); RefreshHint(); ApplyAvailability(); }
        private void ApplyAvailability()
        {
            bool available = GameLocalization.IsReady && !GameSession.IsLoading && !GameInput.IsRebinding; previousAvailable = available;
            foreach (Selectable control in modalControls) if (control != null) control.interactable = available;
            if (fullscreenButton != null) fullscreenButton.interactable = available && GamePreferences.CanChangeWindowMode;
            // Mouse buttons are valid keyboard/mouse bindings, so clicking a
            // cancel button could itself become the binding. That scheme uses
            // the explicitly displayed Esc/timeout cancellation instead.
            if (cancelRebind != null) { cancelRebind.gameObject.SetActive(GameInput.IsRebinding && bindingsGamepad); cancelRebind.interactable = GameInput.IsRebinding; }
            ApplyRunAvailability(available);
            if (gear != null) { gear.gameObject.SetActive(!GameSession.IsOver && !GameSession.IsLoading); gear.interactable = available; }
        }
        private void RefreshFullscreen()
        {
            if (fullscreenValue != null) fullscreenValue.text = GameLocalization.Get(!GamePreferences.CanChangeWindowMode ? "settings.playerWindow" : GamePreferences.FullscreenRequested ? "settings.borderless" : "settings.windowed");
        }
        private void Update()
        {
            UpdateRunFlow();
            if (safeArea != Screen.safeArea) UpdateSafeArea();
            if (lastNavigationFrame != Time.frameCount && !GameInput.ShortcutsSuppressed && !GameSession.IsOver && !GameSession.IsLoading && GameInput.PausePressed && !dropdownWasExpanded)
            {
                lastNavigationFrame = Time.frameCount;
                if (page == "settings" || page == "controls" || page == "credits" || page == "confirmNew" || page == "confirmRetry" || page == "briefing") Back(); else if (!isMenu) GameSession.SetPaused(!GameSession.IsPaused);
            }
            dropdownWasExpanded = languageState != null && languageState.IsOpen;
            if (feedback != null && feedback.text.Length > 0 && Time.unscaledTime >= GameplayFeedback.ExpiresAt) RefreshFeedback();
            if (itemHint != null && Time.unscaledTime >= tutorialUntil && tutorialUntil > 0) { tutorialUntil = 0; RefreshHint(); }
            PlayerInteractionTarget target = interactor != null ? interactor.Target : null;
            if (target != previousTarget || (target != null && target.PromptKey != previousPrompt)) RefreshPrompt();
            if (objectiveGroup != null) objectiveGroup.alpha = Mathf.Clamp01(objectiveUntil - Time.unscaledTime);
            if (bossRoot != null)
            {
                bool nearby = teacher != null && player != null && Mathf.Abs(teacher.transform.position.x - player.transform.position.x) < 15 && Mathf.Abs(teacher.transform.position.y - player.transform.position.y) < 6;
                bossRoot.SetActive(nearby || Time.unscaledTime < bossUntil);
            }
            if (inventoryGroup != null)
            {
                if (!GameSession.BlocksGameplay && GameInput.InventoryPressed) { inventoryPinned = !inventoryPinned; UIAudio.Click(); }
                bool show = inventoryPinned || hoveredSlot >= 0 || Time.unscaledTime < inventoryUntil || (player != null && player.Sleep == 0);
                inventoryGroup.alpha = Mathf.MoveTowards(inventoryGroup.alpha, show ? 1 : 0, Time.unscaledDeltaTime * 5);
                inventoryGroup.blocksRaycasts = inventoryGroup.alpha > 0.05f && !GameSession.BlocksGameplay;
                if (show != inventoryVisible) { inventoryVisible = show; RefreshHint(); }
            }
            bool available = GameLocalization.IsReady && !GameSession.IsLoading && !GameInput.IsRebinding;
            if (available != previousAvailable) ApplyAvailability();
        }
        private void UpdateSafeArea()
        {
            safeArea = Screen.safeArea; if (Screen.width <= 0 || Screen.height <= 0) return;
            safeRoot.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height); safeRoot.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
        }
        private void OnDestroy()
        {
            GameLocalization.Changed -= RefreshLanguage; GamePreferences.Changed -= RefreshLanguage; GameSession.Changed -= RefreshSession; GameplayFeedback.Changed -= RefreshFeedback; GameInput.Changed -= OnInputChanged;
            RunSaveService.Changed -= RefreshSave;
            GameInput.CancelRebind();
            if (player != null) player.StatsChanged -= RefreshStats; if (inventory != null) inventory.Changed -= InventoryChanged;
            if (exams != null) exams.Changed -= BindTeacher; if (teacher != null) teacher.HealthChanged -= TeacherHit;
            GamePreferences.Save(); if (solid != null) Destroy(solid);
        }
        private RectTransform Rect(string name, Transform parent, Vector2 min, Vector2 max, Vector2 position, Vector2 size)
        {
            RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.anchorMin = min; rect.anchorMax = max; rect.anchoredPosition = position; rect.sizeDelta = size; return rect;
        }
        private static void Anchor(RectTransform rect, Vector2 anchor, Vector2 position) { rect.anchorMin = rect.anchorMax = anchor; rect.anchoredPosition = position; }
        private RectTransform Box(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, Color color)
        {
            RectTransform rect = Rect(name, parent, anchor, anchor, position, size); Image image = rect.gameObject.AddComponent<Image>(); image.sprite = solid; image.color = color; return rect;
        }
        private Text TextAt(Transform parent, string key, Vector2 position, Vector2 size, int fontSize)
        {
            RectTransform rect = Rect(string.IsNullOrEmpty(key) ? "Text" : key, parent, Center, Center, position, size);
            Text text = rect.gameObject.AddComponent<Text>(); text.font = font; text.fontSize = fontSize; text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter; text.raycastTarget = false; text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true; text.resizeTextMinSize = 13; text.resizeTextMaxSize = fontSize;
            Shadow shadow = rect.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(0, 0, 0, 0.8f); shadow.effectDistance = new Vector2(1, -1);
            labels.Add(new Label { Text = text, Key = key, Size = fontSize }); return text;
        }
        private Button ButtonAt(Transform parent, string key, Vector2 anchor, Vector2 position, Vector2 size, Action action, int fontSize = 24)
        {
            RectTransform rect = Box(key ?? "Button", parent, anchor, position, size, Surface); Button button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = rect.GetComponent<Image>();
            ColorBlock colors = button.colors; colors.highlightedColor = Accent; colors.selectedColor = Accent; button.colors = colors;
            button.onClick.AddListener(() => { UIAudio.Click(); action(); }); button.gameObject.AddComponent<UIControlSound>().Cancel = HandleCancel;
            Text text = TextAt(rect, key, Vector2.zero, size, fontSize); text.rectTransform.anchorMin = Vector2.zero; text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(8, 3); text.rectTransform.offsetMax = new Vector2(-8, -3);
            if (shade != null && rect.IsChildOf(shade.transform)) modalControls.Add(button); return button;
        }
        private Button IconButton(Transform parent, Vector2 anchor, Vector2 position, bool settings, Action action)
        {
            Button button = ButtonAt(parent, null, anchor, position, new Vector2(44, 44), action, 16); button.gameObject.name = settings ? "Settings" : "Back";
            if (settings)
            {
                for (int i = 0; i < 16; i++)
                {
                    float a = i * Mathf.PI * 2 / 16, b = (i + 1) * Mathf.PI * 2 / 16;
                    Stroke(button.transform, new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * 9, new Vector2(Mathf.Cos(b), Mathf.Sin(b)) * 9, 2);
                }
                for (int i = 0; i < 8; i++) { float a = i * Mathf.PI / 4; Vector2 d = new Vector2(Mathf.Cos(a), Mathf.Sin(a)); Stroke(button.transform, d * 10, d * 15, 3); }
            }
            else { Stroke(button.transform, new Vector2(10, 0), new Vector2(-10, 0), 2); Stroke(button.transform, new Vector2(-10, 0), new Vector2(-1, 9), 2); Stroke(button.transform, new Vector2(-10, 0), new Vector2(-1, -9), 2); }
            return button;
        }
        private void Stroke(Transform parent, Vector2 from, Vector2 to, float width)
        {
            RectTransform line = Box("Icon", parent, Center, (from + to) * 0.5f, new Vector2((to - from).magnitude, width), Color.white);
            line.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg); line.GetComponent<Image>().raycastTarget = false;
        }
        private DefaultControls.Resources StandardResources()
        {
            return new DefaultControls.Resources { standard = solid, background = solid, inputField = solid, knob = solid, checkmark = solid, dropdown = solid, mask = solid };
        }
        private Dropdown DropdownAt(Transform parent, Vector2 position)
        {
            GameObject root = DefaultControls.CreateDropdown(StandardResources()); root.transform.SetParent(parent, false);
            RectTransform rect = root.GetComponent<RectTransform>(); Anchor(rect, Center, position); rect.sizeDelta = new Vector2(360, 48); root.GetComponent<Image>().color = Surface;
            Dropdown dropdown = root.GetComponent<Dropdown>(); dropdown.ClearOptions();
            languageState = root.AddComponent<DropdownOpenState>();
            dropdown.template.gameObject.AddComponent<DropdownListMarker>().Owner = languageState;
            foreach (Text text in root.GetComponentsInChildren<Text>(true)) { text.font = font; text.color = Color.white; text.fontSize = 21; }
            dropdown.template.GetComponent<Image>().color = new Color(0.04f, 0.08f, 0.1f, 0.97f);
            Transform arrow = root.transform.Find("Arrow"); if (arrow != null) arrow.gameObject.SetActive(false);
            RectTransform chevron = Rect("Arrow", root.transform, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-18, 0), Vector2.zero);
            Stroke(chevron, new Vector2(-5, 3), new Vector2(0, -3), 2); Stroke(chevron, new Vector2(0, -3), new Vector2(5, 3), 2);
            root.AddComponent<UIControlSound>().Cancel = () =>
            {
                if (!dropdownWasExpanded && !languageState.IsOpen) HandleCancel();
            };
            foreach (Toggle item in root.GetComponentsInChildren<Toggle>(true))
            {
                item.gameObject.AddComponent<UIControlSound>();
                if (item.targetGraphic != null) item.targetGraphic.color = Surface;
                if (item.graphic != null) item.graphic.color = Accent;
                ColorBlock colors = item.colors;
                colors.normalColor = new Color(1, 1, 1, 0.12f); colors.highlightedColor = Accent; colors.selectedColor = Accent; item.colors = colors;
                RectTransform itemRect = item.GetComponent<RectTransform>(); itemRect.sizeDelta = new Vector2(0, 36);
            }
            dropdown.template.GetComponent<ScrollRect>().content.sizeDelta = new Vector2(0, 42);
            dropdown.template.sizeDelta = new Vector2(0, 264);
            modalControls.Add(dropdown); return dropdown;
        }
        private Slider SliderAt(Transform parent, Vector2 position, float min, float max, float value, Action<float> action)
        {
            RectTransform rect = Box("Slider", parent, Center, position, new Vector2(360, 42), Surface); Slider slider = rect.gameObject.AddComponent<Slider>();
            Image fill = Box("Fill", rect, Center, Vector2.zero, new Vector2(-24, -34), Accent).GetComponent<Image>(); fill.type = Image.Type.Filled; fill.fillMethod = Image.FillMethod.Horizontal; fill.fillOrigin = 0;
            RectTransform handle = Box("Handle", rect, Center, Vector2.zero, new Vector2(16, -12), Color.white);
            slider.fillRect = fill.rectTransform; slider.handleRect = handle; slider.targetGraphic = handle.GetComponent<Image>();
            slider.minValue = min; slider.maxValue = max; slider.SetValueWithoutNotify(value); slider.onValueChanged.AddListener(v => action(v));
            rect.gameObject.AddComponent<UIControlSound>().Cancel = HandleCancel; modalControls.Add(slider); return slider;
        }
    }
}
