using System;
using StudentSurvival.Controls;
using StudentSurvival.Localization;
using StudentSurvival.World;
using UnityEngine;
using UnityEngine.UI;

namespace StudentSurvival.UI
{
    public sealed partial class GameUI
    {
        [Serializable] private sealed class CreditCatalog { public CreditSection[] sections; public string pendingAttributionKey; }
        [Serializable] private sealed class CreditSection { public string id, titleKey; public CreditEntry[] entries; }
        [Serializable] private sealed class CreditEntry { public string name, nameKey, detailKey, source; }
        private GameObject confirmNewPage, confirmRetryPage, creditsPage, briefingPage;
        private Button cancelNew, cancelRetry, creditsBack, briefingReady, briefingBack, menuCreditsButton;
        private bool firstRunBriefing;
        private RectTransform helpContent, creditsContent;
        private Text helpSupplies, helpLabs;
        private float guideUntil;
        private string previousWarning, previousSaveMessage;
        private Text saveStatus, attackWarning, routeText;
        private RawImage titleArtwork;
        private Text titleArtValue, energyStatus;
        private string previousEnergyKey;
        private int previousEnergySeconds = -1;
        private readonly System.Collections.Generic.List<Transform> labPickups = new System.Collections.Generic.List<Transform>();
        private RectTransform labMarker;
        private Camera worldCamera;
        private ObjectiveRoute route;
        private float nextRouteUpdate, savedUntil;
        private void BuildRunFlow()
        {
            TextAt(menuPage.transform, "", new Vector2(0, -300), new Vector2(400, 30), 17).text = "v" + Application.version;
            menuCreditsButton = ButtonAt(menuPage.transform, "credits.title", Center, new Vector2(0, -174), new Vector2(440, 52), OpenCredits);
            confirmNewPage = Page("Confirm new run", false);
            TextAt(confirmNewPage.transform, "save.replaceTitle", new Vector2(0, 145), new Vector2(850, 70), 34);
            TextAt(confirmNewPage.transform, "save.replaceBody", new Vector2(0, 35), new Vector2(820, 110), 23);
            cancelNew = IconButton(confirmNewPage.transform, new Vector2(0, 1), new Vector2(35, -35), false, Back);
            ButtonAt(confirmNewPage.transform, "save.newRun", Center, new Vector2(0, -110), new Vector2(400, 52), GameSession.NewRun);
            confirmRetryPage = Page("Confirm retry", false);
            TextAt(confirmRetryPage.transform, "save.retry", new Vector2(0, 145), new Vector2(850, 70), 34);
            TextAt(confirmRetryPage.transform, "save.retryBody", new Vector2(0, 35), new Vector2(820, 110), 23);
            cancelRetry = IconButton(confirmRetryPage.transform, new Vector2(0, 1), new Vector2(35, -35), false, Back);
            ButtonAt(confirmRetryPage.transform, "save.retry", Center, new Vector2(0, -110), new Vector2(400, 52), GameSession.RetryCheckpoint);
            briefingPage = Page("How to play", false);
            briefingBack = IconButton(briefingPage.transform, new Vector2(0, 1), new Vector2(35, -35), false, Back);
            TextAt(briefingPage.transform, "menu.helpTitle", new Vector2(0, 282), new Vector2(800, 55), 32);
            helpContent = FlowContent(briefingPage.transform, new Vector2(0, -5), new Vector2(870, 475));
            FlowText(helpContent, "help.goalTitle", null, 24, Accent);
            FlowText(helpContent, "help.goalBody", null, 16, Color.white);
            FlowText(helpContent, "help.suppliesTitle", null, 24, Accent);
            helpSupplies = FlowText(helpContent, null, "", 16, Color.white);
            FlowText(helpContent, "help.labsTitle", null, 24, Accent);
            helpLabs = FlowText(helpContent, null, "", 16, Color.white);
            FlowText(helpContent, "help.energyTitle", null, 24, Accent);
            FlowText(helpContent, "help.energyBody", null, 16, Color.white);
            FlowText(helpContent, "help.savesTitle", null, 24, Accent);
            FlowText(helpContent, "help.savesBody", null, 16, Color.white);
            FlowText(helpContent, "help.flirtTitle", null, 24, Accent);
            FlowText(helpContent, "help.flirtBody", null, 16, Color.white);
            briefingReady = ButtonAt(briefingPage.transform, "briefing.ready", Center, new Vector2(0, -292), new Vector2(400, 48), Back);
            creditsPage = Page("Credits", false);
            TextAt(creditsPage.transform, "credits.title", new Vector2(0, 282), new Vector2(800, 55), 32);
            creditsContent = FlowContent(creditsPage.transform, new Vector2(0, -10), new Vector2(870, 490));
            creditsBack = IconButton(creditsPage.transform, new Vector2(0, 1), new Vector2(35, -35), false, Back);
            BuildCreditsFlow();
            saveStatus = TextAt(safeRoot, "", Vector2.zero, new Vector2(300, 72), 16); saveStatus.alignment = TextAnchor.MiddleRight; Anchor(saveStatus.rectTransform, new Vector2(1, 1), new Vector2(-210, -52));
            SettingLabel("settings.titleArt", -210);
            titleArtValue = ButtonAt(settingsPage.transform, null, Center, new Vector2(200, -210), new Vector2(360, 48), GamePreferences.ToggleTitleArt, 21).GetComponentInChildren<Text>();
            guideUntil = Time.time + 90;
            if (hud != null)
            {
                attackWarning = TextAt(hud.transform, "", Vector2.zero, new Vector2(700, 64), 28); attackWarning.color = new Color(1, .8f, .35f);
                Anchor(attackWarning.rectTransform, new Vector2(.5f, 1), new Vector2(0, -124));
                energyStatus = TextAt(hud.transform, "", Vector2.zero, new Vector2(370, 72), 20); energyStatus.alignment = TextAnchor.MiddleLeft;
                Anchor(energyStatus.rectTransform, new Vector2(0, 1), new Vector2(205, -140));
                labMarker = Rect("Nearest lab marker", hud.transform, Center, Center, Vector2.zero, new Vector2(28, 28)); PixelArrow(labMarker, true, Accent); labMarker.gameObject.SetActive(false);
                routeText = TextAt(hud.transform, "", Vector2.zero, new Vector2(600, 64), 24);
                Anchor(routeText.rectTransform, new Vector2(.5f, 1), new Vector2(0, -124));
            }
            if (isMenu)
            {
                Texture2D art = LoadTitleArt();
                if (art != null)
                {
                    RectTransform backdrop = Rect("Student title art", transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                    backdrop.SetAsFirstSibling(); titleArtwork = backdrop.gameObject.AddComponent<RawImage>(); titleArtwork.texture = art; titleArtwork.raycastTarget = false;
                    AspectRatioFitter aspect = backdrop.gameObject.AddComponent<AspectRatioFitter>(); aspect.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent; aspect.aspectRatio = (float)art.width / art.height;
                }
            }
            RefreshRunLanguage(); RefreshSave();
        }
        private static Texture2D LoadTitleArt()
        {
            Texture2D art = Resources.Load<Texture2D>(GamePreferences.MinimalTitleArt ? "Art/StudentTitleMinimal" : "Art/StudentTitle");
            return art != null ? art : Resources.Load<Texture2D>("Art/StudentTitle");
        }
        private void RefreshTitleArt()
        {
            if (titleArtValue != null) titleArtValue.text = GameLocalization.Get(GamePreferences.MinimalTitleArt ? "art.minimal" : "art.illustrated");
            if (titleArtwork == null) return;
            Texture2D art = LoadTitleArt();
            if (art == null || titleArtwork.texture == art) return;
            titleArtwork.texture = art;
            titleArtwork.GetComponent<AspectRatioFitter>().aspectRatio = (float)art.width / art.height;
        }
        private void QuitGame() { if (!isMenu && !RunSaveService.SaveNow()) { RefreshSave(); return; } GamePreferences.Save(); Application.Quit(); }
        private void ConfirmNewRun()
        {
            if (!RunSaveService.HasContinue && !RunSaveService.HasCheckpoint) { GameSession.NewRun(); return; }
            history.Push(page); ShowPage("confirmNew");
        }
        private void ConfirmRetry() { history.Push(page); ShowPage("confirmRetry"); }
        private void OpenBriefing() { firstRunBriefing = false; history.Push(page); ShowPage("briefing"); }
        private void OpenCredits() { history.Push(page); ShowPage("credits"); }
        private void ShowRunPage(string next)
        {
            if (confirmNewPage == null) return;
            confirmNewPage.SetActive(next == "confirmNew"); confirmRetryPage.SetActive(next == "confirmRetry");
            creditsPage.SetActive(next == "credits"); briefingPage.SetActive(next == "briefing");
            briefingReady.gameObject.SetActive(firstRunBriefing); briefingBack.gameObject.SetActive(!firstRunBriefing);
        }
        private void RefreshSave()
        {
            savedUntil = Time.unscaledTime + 2;
            ApplyAvailability();
            // Recovery must remain accessible even if restoring a damaged save was blocked.
            if (RunSaveService.IsRestoring && !string.IsNullOrEmpty(RunSaveService.LastErrorKey) && !isMenu && !GameSession.IsLoading)
            { history.Clear(); ShowPage("pause"); }
        }
        private void ApplyRunAvailability(bool available)
        {
            if (firstMenu != null)
            {
                bool hasContinue = RunSaveService.HasContinue;
                firstMenu.gameObject.SetActive(hasContinue); firstMenu.interactable = available && hasContinue;
                float y = 82;
                foreach (Button action in new[] { firstMenu, newRunButton, menuHelpButton, menuControlsButton, menuCreditsButton, menuQuitButton })
                { if (action == null || !action.gameObject.activeSelf) continue; Anchor(action.GetComponent<RectTransform>(), Center, new Vector2(0, y)); y -= 64; }
            }
            if (firstResult != null) firstResult.interactable = available && (GameSession.Result == SessionResult.Won || RunSaveService.HasCheckpoint);
        }
        private void RefreshRunLanguage()
        {
            previousWarning = previousSaveMessage = previousEnergyKey = null; nextRouteUpdate = 0; RefreshTitleArt();
            if (helpSupplies != null) helpSupplies.text = GameLocalization.Get("help.suppliesBody", GameInput.Display("Action"));
            if (helpLabs != null) helpLabs.text = GameLocalization.Get("help.labsBody", GameInput.Display("Fire"), GameInput.Display("UseItem"));
            if (firstResult != null) firstResult.GetComponentInChildren<Text>().text = GameLocalization.Get(GameSession.Result == SessionResult.Won ? "save.newRun" : "save.retry");
            if (helpContent != null) LayoutRebuilder.ForceRebuildLayoutImmediate(helpContent);
            if (creditsContent != null) LayoutRebuilder.ForceRebuildLayoutImmediate(creditsContent);
        }
        private RectTransform FlowContent(Transform parent, Vector2 position, Vector2 size)
        {
            GameObject root = DefaultControls.CreateScrollView(StandardResources()); root.transform.SetParent(parent, false);
            RectTransform rect = root.GetComponent<RectTransform>(); Anchor(rect, Center, position); rect.sizeDelta = size;
            root.GetComponent<Image>().color = new Color(.025f, .045f, .055f, .72f);
            ScrollRect scroll = root.GetComponent<ScrollRect>(); scroll.horizontal = false; scroll.scrollSensitivity = 32;
            if (scroll.horizontalScrollbar != null) scroll.horizontalScrollbar.gameObject.SetActive(false);
            foreach (Scrollbar bar in root.GetComponentsInChildren<Scrollbar>())
            { bar.GetComponent<Image>().color = Glass; if (bar.targetGraphic != null) bar.targetGraphic.color = Accent; ControlFeedback(bar.gameObject); modalControls.Add(bar); }
            RectTransform content = scroll.content; content.anchorMin = Vector2.up; content.anchorMax = Vector2.one; content.pivot = Vector2.up;
            content.sizeDelta = Vector2.zero;
            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 28, 24, 24); layout.spacing = 28; layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = layout.childControlHeight = true; layout.childForceExpandWidth = true; layout.childForceExpandHeight = false;
            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>(); fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return content;
        }
        private Text FlowText(Transform parent, string key, string value, int size, Color color)
        {
            size = size >= 22 ? 32 : 24;
            Text text = TextAt(parent, key, Vector2.zero, Vector2.zero, size);
            if (key == null) text.text = value;
            text.resizeTextForBestFit = false; text.alignment = TextAnchor.UpperLeft; text.lineSpacing = 1.5f;
            Shadow shadow = text.GetComponent<Shadow>(); if (shadow != null) shadow.enabled = false;
            text.verticalOverflow = VerticalWrapMode.Overflow; text.color = color;
            return text;
        }
        private void BuildCreditsFlow()
        {
            TextAsset asset = Resources.Load<TextAsset>("Credits");
            if (asset == null) return;
            CreditCatalog catalog = JsonUtility.FromJson<CreditCatalog>(asset.text);
            if (catalog.sections != null) foreach (CreditSection section in catalog.sections)
            {
                FlowText(creditsContent, section.titleKey, null, 24, Accent);
                if (section.entries == null) continue;
                foreach (CreditEntry entry in section.entries)
                {
                    FlowText(creditsContent, string.IsNullOrEmpty(entry.nameKey) ? null : entry.nameKey, entry.name, 16, Color.white);
                    if (section.id != "team") FlowText(creditsContent, entry.detailKey, null, 16, new Color(.72f, .8f, .79f));
                }
            }
            FlowText(creditsContent, "credits.generatedArt", null, 16, new Color(.72f, .8f, .79f));
            FlowText(creditsContent, catalog.pendingAttributionKey, null, 16, new Color(1, .8f, .45f));
        }

        private void CacheLabPickups()
        {
            worldCamera = Camera.main;
            foreach (StudentSurvival.Inventory.LootBoxItem pickup in FindObjectsByType<StudentSurvival.Inventory.LootBoxItem>(FindObjectsSortMode.None))
                if (pickup.ItemKey == "item.Lab") labPickups.Add(pickup.transform);
        }
        private Transform FindNearestLab()
        {
            if (inventory != null)
                for (int i = 0; i < GameInput.SlotCount; i++)
                    if (inventory.ItemAt(i) != null && inventory.ItemAt(i).itemType == StudentSurvival.Inventory.Item.Lab) return null;
            Transform nearest = null; float best = float.PositiveInfinity;
            foreach (Transform pickup in labPickups)
            {
                if (pickup == null || !pickup.gameObject.activeInHierarchy) continue;
                float distance = (pickup.position - player.transform.position).sqrMagnitude;
                if (distance < best) { best = distance; nearest = pickup; }
            }
            return nearest;
        }
        private void UpdateRunFlow()
        {
            if (energyStatus != null && player != null)
            {
                string key = player.EnergyStatusKey; int seconds = Mathf.CeilToInt(player.EnergyStatusSeconds);
                if (key != previousEnergyKey || seconds != previousEnergySeconds)
                {
                    previousEnergyKey = key; previousEnergySeconds = seconds;
                    energyStatus.text = GameLocalization.Get(key, seconds, GameInput.Display("UseItem"));
                    energyStatus.color = key == "energy.exhausted" ? new Color(1, .5f, .35f) : key == "energy.crash" ? new Color(1, .8f, .35f) : Accent;
                }
            }
            string saveKey = !string.IsNullOrEmpty(RunSaveService.LastErrorKey) ? RunSaveService.LastErrorKey : Time.unscaledTime < savedUntil && !isMenu ? "save.saved" : "";
            if (saveStatus != null && saveKey != previousSaveMessage) { previousSaveMessage = saveKey; saveStatus.text = GameLocalization.Get(saveKey); }
            string warningKey = teacher != null && teacher.IsTelegraphing ? teacher.AttackWarningKey : player != null && player.IsInvulnerable ? "combat.protected" : "";
            if (attackWarning != null && warningKey != previousWarning)
            { previousWarning = warningKey; attackWarning.text = GameLocalization.Get(warningKey, GameInput.Display(warningKey == "combat.crouch" ? "Crouch" : "Jump")); }
            if (routeText == null || player == null || route == null || GameSession.BlocksGameplay || Time.unscaledTime < nextRouteUpdate) return;
            nextRouteUpdate = Time.unscaledTime + .4f;
            if (teacher == null || Time.time > guideUntil) { routeText.text = string.Empty; if (labMarker != null) labMarker.gameObject.SetActive(false); return; }
            bool hideGuide = !string.IsNullOrEmpty(player.EnergyStatusKey) || (teacher != null && teacher.IsTelegraphing);
            routeText.gameObject.SetActive(!hideGuide);
            if (hideGuide) { if (labMarker != null) labMarker.gameObject.SetActive(false); return; }
            Transform lab = FindNearestLab();
            bool nearbyTeacher = Vector2.Distance(player.transform.position, teacher.transform.position) < 12;
            Vector3 goal = lab != null && !nearbyTeacher ? lab.position : teacher.transform.position;
            Vector3 destination = route.Next(player.transform.position, goal);
            string arrow = Mathf.Abs(destination.x - player.transform.position.x) < 1.5f ? "↓" : destination.x < player.transform.position.x ? "←" : "→";
            routeText.text = lab != null && !nearbyTeacher ? GameLocalization.Get("guide.findLabs", arrow, GameInput.Display("Action"))
                : nearbyTeacher ? GameLocalization.Get("guide.throwBrief", GameInput.Display("Fire"))
                : GameLocalization.Get("guide.exam", arrow, GameInput.Display("Action"));
            if (labMarker != null)
            {
                bool showMarker = lab != null && !nearbyTeacher && worldCamera != null;
                labMarker.gameObject.SetActive(showMarker);
                if (showMarker)
                {
                    Vector3 screen = worldCamera.WorldToScreenPoint(lab.position + Vector3.up * 1.1f);
                    bool visible = screen.z > 0 && screen.x >= 0 && screen.x <= Screen.width && screen.y >= 0 && screen.y <= Screen.height;
                    labMarker.gameObject.SetActive(visible);
                    if (visible && RectTransformUtility.ScreenPointToLocalPointInRectangle(safeRoot, screen, null, out Vector2 local)) labMarker.anchoredPosition = local;
                }
            }

        }
    }
}
