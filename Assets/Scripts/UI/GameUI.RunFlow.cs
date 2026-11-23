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
        [Serializable] private sealed class CreditEntry { public string name, nameKey, detailKey, source, githubUrl; }
        private GameObject confirmNewPage, confirmRetryPage, creditsPage, briefingPage;
        private Button cancelNew, cancelRetry, creditsBack, briefingReady, briefingBack, menuCreditsButton;
        private bool firstRunBriefing;
        private RectTransform helpContent, creditsContent;
        private Text helpSupplies, helpLabs;
        private string previousWarning, previousSaveMessage;
        private Text saveStatus, attackWarning, routeText;
        private RawImage titleArtwork;
        private Text titleArtValue, energyStatus;
        private string previousEnergyKey;
        private int previousEnergySeconds = -1;
        private readonly System.Collections.Generic.List<Transform> labPickups = new System.Collections.Generic.List<Transform>();
        private RectTransform labMarker, flirtHintRoot;
        private Text flirtHintText;
        private Camera worldCamera;
        private Transform markedLab;
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
            FlowText(helpContent, "campaign.helpGoal", null, 16, Color.white);
            FlowText(helpContent, "help.suppliesTitle", null, 24, Accent);
            helpSupplies = FlowText(helpContent, null, "", 16, Color.white);
            FlowText(helpContent, "help.labsTitle", null, 24, Accent);
            helpLabs = FlowText(helpContent, null, "", 16, Color.white);
            FlowText(helpContent, "help.teacherCue", null, 17, Color.white);
            FlowText(helpContent, "help.energyTitle", null, 24, Accent);
            FlowText(helpContent, "help.energyBody", null, 16, Color.white);
            FlowText(helpContent, "help.savesTitle", null, 24, Accent);
            FlowText(helpContent, "help.savesBody", null, 16, Color.white);
            FlowText(helpContent, "campaign.helpExamTitle", null, 24, Accent);
            FlowText(helpContent, "campaign.helpExamBodyV2", null, 16, Color.white);
            FlowText(helpContent, "campaign.helpCheatTitle", null, 24, Accent);
            FlowText(helpContent, "campaign.helpCheatBodyV2", null, 16, Color.white);
            FlowText(helpContent, "campaign.helpSocialTitle", null, 24, Accent);
            FlowText(helpContent, "campaign.helpSocialBody", null, 16, Color.white);
            briefingReady = ButtonAt(briefingPage.transform, "briefing.ready", Center, new Vector2(0, -292), new Vector2(400, 48), Back);
            creditsPage = Page("Credits", false);
            TextAt(creditsPage.transform, "credits.title", new Vector2(0, 282), new Vector2(800, 55), 32);
            creditsContent = FlowContent(creditsPage.transform, new Vector2(0, -10), new Vector2(870, 490));
            creditsBack = IconButton(creditsPage.transform, new Vector2(0, 1), new Vector2(35, -35), false, Back);
            BuildCreditsFlow();
            saveStatus = TextAt(safeRoot, "", Vector2.zero, new Vector2(300, 72), 16); saveStatus.alignment = TextAnchor.MiddleRight; Anchor(saveStatus.rectTransform, new Vector2(1, 1), new Vector2(-210, -52));
            SettingLabel("settings.titleArt", -210);
            titleArtValue = ButtonAt(settingsPage.transform, null, Center, new Vector2(200, -210), new Vector2(360, 48), GamePreferences.ToggleTitleArt, 21).GetComponentInChildren<Text>();
            if (hud != null)
            {
                attackWarning = TextAt(hud.transform, "", Vector2.zero, new Vector2(700, 64), 28); attackWarning.color = new Color(1, .8f, .35f);
                Anchor(attackWarning.rectTransform, new Vector2(.5f, 1), new Vector2(150, -124));
                energyStatus = TextAt(hud.transform, "", Vector2.zero, new Vector2(370, 72), 20); energyStatus.alignment = TextAnchor.MiddleLeft;
                Anchor(energyStatus.rectTransform, new Vector2(0, 1), new Vector2(205, -140));
                flirtHintRoot = Box("Nearby flirt hint", hud.transform, Center, Vector2.zero, new Vector2(800, 90), new Color(.035f, .045f, .055f, .86f));
                flirtHintRoot.GetComponent<Image>().raycastTarget = false;
                RectTransform pinkEdge = Box("Pink pixel edge", flirtHintRoot, new Vector2(.5f, 1), new Vector2(0, -2), new Vector2(800, 4), new Color(1f, .34f, .55f));
                pinkEdge.anchorMin = new Vector2(0, 1); pinkEdge.anchorMax = Vector2.one; pinkEdge.sizeDelta = new Vector2(0, 4);
                pinkEdge.GetComponent<Image>().raycastTarget = false;
                flirtHintText = TextAt(flirtHintRoot, null, Vector2.zero, new Vector2(776, 70), 24);
                flirtHintText.rectTransform.anchorMin = Vector2.zero; flirtHintText.rectTransform.anchorMax = Vector2.one;
                flirtHintText.rectTransform.offsetMin = new Vector2(12, 10); flirtHintText.rectTransform.offsetMax = new Vector2(-12, -10);
                flirtHintText.resizeTextForBestFit = false; flirtHintText.lineSpacing = 1.25f;
                flirtHintText.GetComponent<Shadow>().enabled = false;
                flirtHintRoot.gameObject.SetActive(false);
                labMarker = Rect("Nearest lab marker", hud.transform, Center, Center, Vector2.zero, new Vector2(28, 28)); PixelArrow(labMarker, true, Accent); labMarker.gameObject.SetActive(false);
                routeText = TextAt(hud.transform, "", Vector2.zero, new Vector2(780, 96), 24);
                Anchor(routeText.rectTransform, new Vector2(.5f, 1), new Vector2(160, -140));
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
            if (!RunSaveService.HasContinue && !RunSaveService.HasCheckpoint && GameSession.Result != SessionResult.Won) { GameSession.NewRun(); return; }
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
            if (resultNewRun != null) { resultNewRun.gameObject.SetActive(GameSession.Result == SessionResult.Won); resultNewRun.interactable = available; }
            if (resultMenu != null)
            {
                resultMenu.interactable = available;
                Anchor(resultMenu.GetComponent<RectTransform>(), Center, new Vector2(0, GameSession.Result == SessionResult.Won ? -198 : -134));
            }
        }
        private void RefreshRunLanguage()
        {
            previousWarning = previousSaveMessage = previousEnergyKey = null; nextRouteUpdate = 0; RefreshTitleArt();
            if (helpSupplies != null) helpSupplies.text = GameLocalization.Get("help.suppliesBodyClear", GameInput.Display("Action"));
            if (helpLabs != null) helpLabs.text = GameLocalization.Get("help.labsBodyClear", GameInput.Display("Fire"), GameInput.Display("UseItem"), GameInput.Display("PreviousSlot"), GameInput.Display("NextSlot"));
            if (firstResult != null) firstResult.GetComponentInChildren<Text>().text = GameLocalization.Get(GameSession.Result == SessionResult.Won ? "campaign.keepExploring" : "save.retry");
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
        private static int FlowFontSize(bool heading)
        {
            float scale = Mathf.Max(.25f, Mathf.Min(Screen.width / 1280f, Screen.height / 720f));
            return Mathf.Clamp(Mathf.CeilToInt((heading ? 26 : 18) / scale / 8) * 8, heading ? 24 : 16, heading ? 48 : 32);
        }
        private Text FlowText(Transform parent, string key, string value, int size, Color color)
        {
            bool heading = size >= 22;
            Text text = TextAt(parent, key, Vector2.zero, Vector2.zero, FlowFontSize(heading));
            labels[labels.Count - 1].Flow = true; labels[labels.Count - 1].Heading = heading;
            if (key == null) text.text = value;
            text.resizeTextForBestFit = false; text.alignment = TextAnchor.UpperLeft; text.lineSpacing = 1.5f;
            Shadow shadow = text.GetComponent<Shadow>(); if (shadow != null) shadow.enabled = false;
            text.verticalOverflow = VerticalWrapMode.Overflow; text.color = color;
            return text;
        }
        private static bool TryGetCreditProfile(string value, out string profile)
        {
            profile = null;
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri uri) || uri.Scheme != Uri.UriSchemeHttps ||
                !string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase) || !uri.IsDefaultPort ||
                uri.UserInfo.Length != 0 || uri.Query.Length != 0 || uri.Fragment.Length != 0) return false;
            string username = uri.AbsolutePath.Trim('/');
            if (username.Length == 0 || username.Length > 39) return false;
            foreach (char c in username)
                if (!(c >= 'a' && c <= 'z') && !(c >= 'A' && c <= 'Z') && !(c >= '0' && c <= '9') && c != '-') return false;
            profile = "https://github.com/" + username;
            return true;
        }
        private void CreditName(CreditEntry entry)
        {
            string key = string.IsNullOrEmpty(entry.nameKey) ? null : entry.nameKey;
            if (!TryGetCreditProfile(entry.githubUrl, out string profile))
            { FlowText(creditsContent, key, entry.name, 16, Color.white); return; }
            Button link = ButtonAt(creditsContent, key, Center, Vector2.zero, Vector2.zero,
                () => Application.OpenURL(profile), FlowFontSize(false));
            link.gameObject.name = "GitHub profile - " + entry.name;
            // Let the same flowing layout used by credit paragraphs measure wrapped names.
            // Fixed-height buttons would clip names when interface text is enlarged.
            VerticalLayoutGroup layout = link.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true; layout.childForceExpandHeight = false;
            link.gameObject.AddComponent<LayoutElement>().minHeight = 56;
            Text text = link.GetComponentInChildren<Text>();
            labels[labels.Count - 1].Flow = true; labels[labels.Count - 1].Heading = false;
            text.text = key == null ? entry.name : GameLocalization.Get(key);
            text.resizeTextForBestFit = false; text.alignment = TextAnchor.MiddleLeft; text.lineSpacing = 1.5f;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.GetComponent<Shadow>().enabled = false;
            link.gameObject.AddComponent<ScrollSelectionIntoView>().Scroll = creditsContent.GetComponentInParent<ScrollRect>();
        }
        private void BuildCreditsFlow()
        {
            TextAsset asset = Resources.Load<TextAsset>("Credits");
            if (asset == null) return;
            CreditCatalog catalog = JsonUtility.FromJson<CreditCatalog>(asset.text);
            if (catalog.sections != null) foreach (CreditSection section in catalog.sections)
            {
                FlowText(creditsContent, section.titleKey, null, 24, Accent);
                if (section.id == "team") FlowText(creditsContent, "credits.profileHint", null, 16, new Color(.72f, .8f, .79f));
                if (section.entries == null) continue;
                foreach (CreditEntry entry in section.entries)
                {
                    CreditName(entry);
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
                StudentSurvival.Characters.CampaignRooms room = StudentSurvival.Characters.ExamProgression.RoomContaining(pickup.position);
                if (room != null && CampaignState.Stage < room.RequiredStage) continue;
                float distance = (pickup.position - player.transform.position).sqrMagnitude;
                if (distance < best) { best = distance; nearest = pickup; }
            }
            return nearest;
        }
        private void UpdateLabMarker()
        {
            if (labMarker == null) return;
            bool visible = markedLab != null && markedLab.gameObject.activeInHierarchy && worldCamera != null && !GameSession.BlocksGameplay;
            if (!visible) { labMarker.gameObject.SetActive(false); return; }
            Vector3 screen = worldCamera.WorldToScreenPoint(markedLab.position + Vector3.up * 1.1f);
            visible = screen.z > 0 && screen.x >= 0 && screen.x <= Screen.width && screen.y >= 0 && screen.y <= Screen.height;
            labMarker.gameObject.SetActive(visible);
            if (visible && RectTransformUtility.ScreenPointToLocalPointInRectangle(safeRoot, screen, null, out Vector2 local)) labMarker.anchoredPosition = local;
        }
        private void UpdateFlirtHint()
        {
            if (flirtHintRoot == null) return;
            PlayerInteractionTarget target = interactor != null ? interactor.Target : null;
            bool visible = target is StudentSurvival.Inventory.FlirtDistraction && target.HasPlayerNearby &&
                worldCamera != null && !GameSession.BlocksGameplay;
            flirtHintRoot.gameObject.SetActive(visible);
            if (!visible) return;
            Vector3 screen = worldCamera.WorldToScreenPoint(target.transform.position + Vector3.up * 3.9f);
            if (screen.z <= 0 || screen.x < 0 || screen.x > Screen.width) { flirtHintRoot.gameObject.SetActive(false); return; }
            float width = Mathf.Min(800 * GamePreferences.TextScale, safeRoot.rect.width - 24);
            float height = Mathf.Max(90, flirtHintText.preferredHeight + 24);
            Vector2 size = new Vector2(width, height);
            if (flirtHintRoot.sizeDelta != size) flirtHintRoot.sizeDelta = size;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(safeRoot, screen, null, out Vector2 local))
            {
                Rect area = safeRoot.rect;
                local.x = Mathf.Clamp(local.x, area.xMin + width * .5f + 12, area.xMax - width * .5f - 12);
                local.y = Mathf.Clamp(local.y, area.yMin + 220 + height * .5f, area.yMax - 105 - height * .5f);
                flirtHintRoot.anchoredPosition = local;
            }
        }
        private void UpdateRunFlow()
        {
            UpdateLabMarker(); UpdateFlirtHint();
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
            StudentSurvival.Inventory.CheatingEncounter guard = StudentSurvival.Inventory.CheatingEncounter.Instance;
            int guardSeconds = guard != null && guard.IsSuspicious ? Mathf.CeilToInt(guard.WarningSeconds)
                : guard != null && guard.IsInspecting ? Mathf.CeilToInt(guard.InspectionSeconds) : -1;
            string guardKey = guard != null && guard.IsSuspicious ? "cheat.guardCountdown" : "cheat.inspection";
            string warningKey = guardSeconds >= 0 ? guardKey : teacher != null && teacher.IsTelegraphing ? teacher.AttackWarningKey : teacher != null && teacher.IsDamageProtected ? "combat.teacherProtected" : player != null && player.IsInvulnerable ? "combat.protected" : "";
            if (attackWarning != null && (warningKey != previousWarning || guardSeconds != previousGuardSeconds))
            {
                previousWarning = warningKey; previousGuardSeconds = guardSeconds;
                attackWarning.text = warningKey == "cheat.guardCountdown" || warningKey == "cheat.inspection" ? GameLocalization.Get(warningKey, guardSeconds)
                    : GameLocalization.Get(warningKey == "combat.crouch" ? "combat.dodgeCrouch" : warningKey == "combat.jump" ? "combat.dodgeJump" : warningKey, GameInput.Display(warningKey == "combat.crouch" ? "Crouch" : "Jump"));
            }
            UpdateCampaignRoute(warningKey);

        }
    }
}
