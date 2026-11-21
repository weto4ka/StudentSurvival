using System;
using System.Text;
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
        [Serializable] private sealed class CreditEntry { public string name, detailKey, source; }
        private GameObject confirmNewPage, confirmRetryPage, creditsPage, briefingPage;
        private Button cancelNew, cancelRetry, creditsBack, briefingReady;
        private float guideUntil;
        private string previousWarning, previousSaveMessage;
        private Text creditsText, saveStatus, attackWarning, routeText, briefingText;
        private RawImage titleArtwork;
        private Text titleArtValue, energyStatus;
        private string previousEnergyKey;
        private int previousEnergySeconds = -1;
        private ObjectiveRoute route;
        private float nextRouteUpdate, savedUntil;
        private void BuildRunFlow()
        {
            TextAt(menuPage.transform, "", new Vector2(0, -300), new Vector2(400, 30), 17).text = "v" + Application.version;
            ButtonAt(menuPage.transform, "credits.title", Center, new Vector2(0, -160), new Vector2(370, 52), OpenCredits);
            confirmNewPage = Page("Confirm new run", true);
            TextAt(confirmNewPage.transform, "save.replaceTitle", new Vector2(0, 145), new Vector2(850, 70), 34);
            TextAt(confirmNewPage.transform, "save.replaceBody", new Vector2(0, 35), new Vector2(820, 110), 23);
            cancelNew = ButtonAt(confirmNewPage.transform, "menu.back", Center, new Vector2(-195, -110), new Vector2(350, 52), Back);
            ButtonAt(confirmNewPage.transform, "save.newRun", Center, new Vector2(195, -110), new Vector2(350, 52), GameSession.NewRun);
            confirmRetryPage = Page("Confirm retry", true);
            TextAt(confirmRetryPage.transform, "save.retry", new Vector2(0, 145), new Vector2(850, 70), 34);
            TextAt(confirmRetryPage.transform, "save.retryBody", new Vector2(0, 35), new Vector2(820, 110), 23);
            cancelRetry = ButtonAt(confirmRetryPage.transform, "menu.back", Center, new Vector2(-195, -110), new Vector2(350, 52), Back);
            ButtonAt(confirmRetryPage.transform, "save.retry", Center, new Vector2(195, -110), new Vector2(350, 52), GameSession.RetryCheckpoint);
            briefingPage = Page("Briefing", true);
            TextAt(briefingPage.transform, "briefing.title", new Vector2(0, 235), new Vector2(820, 70), 36);
            briefingText = TextAt(briefingPage.transform, "", new Vector2(0, 35), new Vector2(830, 280), 25);
            briefingReady = ButtonAt(briefingPage.transform, "briefing.ready", Center, new Vector2(0, -170), new Vector2(370, 52), Back);
            ButtonAt(briefingPage.transform, "menu.controls", Center, new Vector2(0, -240), new Vector2(370, 48), OpenControls);
            creditsPage = Page("Credits", false);
            TextAt(creditsPage.transform, "credits.title", new Vector2(0, 270), new Vector2(800, 60), 34);
            GameObject scroll = DefaultControls.CreateScrollView(StandardResources()); scroll.transform.SetParent(creditsPage.transform, false);
            RectTransform scrollRect = scroll.GetComponent<RectTransform>(); Anchor(scrollRect, Center, new Vector2(0, -5)); scrollRect.sizeDelta = new Vector2(880, 450);
            scroll.GetComponent<Image>().color = Glass;
            ScrollRect creditsScroll = scroll.GetComponent<ScrollRect>(); creditsScroll.horizontal = false; creditsScroll.scrollSensitivity = 35;
            if (creditsScroll.horizontalScrollbar != null) creditsScroll.horizontalScrollbar.gameObject.SetActive(false);
            foreach (Scrollbar scrollbar in scroll.GetComponentsInChildren<Scrollbar>())
            { scrollbar.gameObject.AddComponent<UIControlSound>().Cancel = HandleCancel; modalControls.Add(scrollbar); }
            RectTransform content = creditsScroll.content; content.anchorMin = Vector2.up; content.anchorMax = Vector2.one; content.pivot = Vector2.up;
            creditsText = TextAt(content, "", Vector2.zero, Vector2.zero, 21); creditsText.alignment = TextAnchor.UpperLeft; creditsText.resizeTextForBestFit = false;
            creditsText.verticalOverflow = VerticalWrapMode.Overflow; creditsText.rectTransform.anchorMin = Vector2.zero; creditsText.rectTransform.anchorMax = Vector2.one;
            creditsText.rectTransform.offsetMin = new Vector2(20, 15); creditsText.rectTransform.offsetMax = new Vector2(-25, -15);
            creditsBack = IconButton(creditsPage.transform, new Vector2(0, 1), new Vector2(35, -35), false, Back);
            saveStatus = TextAt(safeRoot, "", Vector2.zero, new Vector2(440, 44), 18); saveStatus.alignment = TextAnchor.MiddleRight; Anchor(saveStatus.rectTransform, new Vector2(1, 1), new Vector2(-280, -32));
            SettingLabel("settings.titleArt", -210);
            titleArtValue = ButtonAt(settingsPage.transform, null, Center, new Vector2(200, -210), new Vector2(360, 48), GamePreferences.ToggleTitleArt, 21).GetComponentInChildren<Text>();
            guideUntil = Time.time + 90;
            if (hud != null)
            {
                attackWarning = TextAt(hud.transform, "", Vector2.zero, new Vector2(900, 55), 30); attackWarning.color = new Color(1, .8f, .35f);
                Anchor(attackWarning.rectTransform, new Vector2(.5f, 1), new Vector2(0, -177));
                energyStatus = TextAt(hud.transform, "", Vector2.zero, new Vector2(370, 54), 18); energyStatus.alignment = TextAnchor.MiddleLeft;
                Anchor(energyStatus.rectTransform, new Vector2(0, 1), new Vector2(205, -80));
                routeText = TextAt(hud.transform, "", Vector2.zero, new Vector2(700, 45), 21);
                Anchor(routeText.rectTransform, new Vector2(.5f, 1), new Vector2(0, -87));
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
        private void OpenBriefing() { history.Push(page); ShowPage("briefing"); }
        private void OpenCredits() { history.Push(page); ShowPage("credits"); }
        private void ShowRunPage(string next)
        {
            if (confirmNewPage == null) return;
            confirmNewPage.SetActive(next == "confirmNew"); confirmRetryPage.SetActive(next == "confirmRetry");
            creditsPage.SetActive(next == "credits"); briefingPage.SetActive(next == "briefing");
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
            if (firstMenu != null) firstMenu.interactable = available && RunSaveService.HasContinue;
            if (firstResult != null) firstResult.interactable = available && (GameSession.Result == SessionResult.Won || RunSaveService.HasCheckpoint);
        }
        private void RefreshRunLanguage()
        {
            previousWarning = previousSaveMessage = previousEnergyKey = null; nextRouteUpdate = 0; RefreshTitleArt();
            if (briefingReady != null) briefingReady.GetComponentInChildren<Text>().text = GameLocalization.Get(isMenu ? "menu.back" : "briefing.ready");
            if (briefingText != null) briefingText.text = GameLocalization.Get("briefing.bodyTimed", GameInput.Display("Action"), GameInput.Display("Fire"), GameInput.Display("UseItem"));
            if (firstResult != null) firstResult.GetComponentInChildren<Text>().text = GameLocalization.Get(GameSession.Result == SessionResult.Won ? "save.newRun" : "save.retry");
            if (creditsText == null) return;
            TextAsset asset = Resources.Load<TextAsset>("Credits"); var body = new StringBuilder();
            if (asset != null)
            {
                CreditCatalog catalog = JsonUtility.FromJson<CreditCatalog>(asset.text);
                if (catalog.sections != null) foreach (CreditSection section in catalog.sections)
                {
                    body.AppendLine(GameLocalization.Get(section.titleKey));
                    if (section.entries != null) foreach (CreditEntry entry in section.entries)
                        body.Append(entry.name).Append(" — ").AppendLine(GameLocalization.Get(entry.detailKey));
                    body.AppendLine();
                }
                body.AppendLine(GameLocalization.Get(catalog.pendingAttributionKey));
            }
            body.AppendLine().AppendLine(GameLocalization.Get("credits.generatedArt"));
            creditsText.text = body.ToString(); Canvas.ForceUpdateCanvases();
            creditsText.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, Mathf.Max(450, creditsText.preferredHeight + 50));
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
            if (teacher == null || Time.time > guideUntil) { routeText.text = string.Empty; return; }
            Vector3 destination = route.Next(player.transform.position, teacher.transform.position);
            string arrow = Mathf.Abs(destination.x - player.transform.position.x) < 1.5f ? "↓" : destination.x < player.transform.position.x ? "←" : "→";
            routeText.text = GameLocalization.Get(GameSession.PickupsCollected == 0 ? "guide.collect" : "guide.exam", arrow, GameInput.Display("Action"));
            if (Vector2.Distance(player.transform.position, teacher.transform.position) < 12) routeText.text = GameLocalization.Get("guide.fight", GameInput.Display("Fire"), GameInput.Display("UseItem"));
        }
    }
}
