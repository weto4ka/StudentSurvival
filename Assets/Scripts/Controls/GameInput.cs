using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StudentSurvival.Controls
{
    public static class GameInput
    {
        public const int SlotCount = 9;
        private const string Preference = "StudentSurvival.BindingOverrides";
        private static InputActionAsset asset;
        private static InputActionMap map;
        private static InputActionRebindingExtensions.RebindingOperation rebind;
        private static bool usingGamepad;
        private static readonly string[] SlotNames = { "Slot1", "Slot2", "Slot3", "Slot4", "Slot5", "Slot6", "Slot7", "Slot8", "Slot9" };
        private static int suppressUntil;
        public static bool IsRebinding { get { return rebind != null; } }
        public static bool ShortcutsSuppressed { get { return IsRebinding || Time.frameCount <= suppressUntil; } }
        public static bool UsingGamepad { get { return usingGamepad; } }
        public static bool RebindConflict { get; private set; }
        public static event Action Changed;
        public static float Horizontal { get { return Action("Move").ReadValue<float>(); } }
        public static bool JumpPressed { get { return Pressed("Jump"); } }
        public static bool CrouchHeld { get { return Action("Crouch").IsPressed(); } }
        public static bool ActionPressed { get { return Pressed("Action"); } }
        public static bool FirePressed { get { return Pressed("Fire"); } }
        public static bool PausePressed { get { return Pressed("Pause"); } }
        public static bool RestartPressed { get { return Pressed("Restart"); } }
        public static bool BackToMenuPressed { get { return PausePressed; } }
        public static bool InventoryHeld { get { return Action("Inventory").IsPressed(); } }
        public static bool InventoryPressed { get { return Pressed("Inventory"); } }
        public static bool PreviousSlotPressed { get { return Pressed("PreviousSlot"); } }
        public static bool NextSlotPressed { get { return Pressed("NextSlot"); } }
        public static bool UseItemPressed { get { return Pressed("UseItem"); } }
        public static bool PointerFirePressed { get { return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame; } }
        public static Vector2 PointerPosition { get { return Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero; } }
        public static bool SlotPressed(int index) { return index >= 0 && index < SlotCount && Pressed(SlotNames[index]); }
        public static string SlotDisplay(int index) { return Action(SlotNames[index]).GetBindingDisplayString(0); }
        private static bool Pressed(string name) { return !ShortcutsSuppressed && Action(name).WasPressedThisFrame(); }
        private static InputAction Action(string name) { Build(); return map.FindAction(name, true); }
        public static string Display(string name, bool? gamepad = null)
        {
            return Action(name).GetBindingDisplayString(group: (gamepad ?? UsingGamepad) ? "Gamepad" : "KeyboardMouse");
        }
        public sealed class BindingOption
        {
            public InputAction Action;
            public int Index;
            public string LabelKey { get { return Action.name.StartsWith("Slot", StringComparison.Ordinal) ? "control.Slot" : Action.name == "Move" && Action.bindings[Index].isPartOfComposite ? "control.Move." + Action.bindings[Index].name : "control." + Action.name; } }
            public int SlotNumber { get { return Action.name.StartsWith("Slot", StringComparison.Ordinal) ? int.Parse(Action.name.Substring(4)) : 0; } }
            public string Display { get { return Action.GetBindingDisplayString(Index); } }
        }
        public static List<BindingOption> Bindings(bool gamepad)
        {
            Build();
            string group = gamepad ? "Gamepad" : "KeyboardMouse";
            var result = new List<BindingOption>();
            foreach (InputAction action in map.actions)
                for (int i = 0; i < action.bindings.Count; i++)
                    if (!action.bindings[i].isComposite && action.bindings[i].groups == group)
                        result.Add(new BindingOption { Action = action, Index = i });
            return result;
        }
        public static void Rebind(BindingOption option, bool gamepad)
        {
            if (IsRebinding) return;
            RebindConflict = false;
            string prior = option.Action.bindings[option.Index].overridePath;
            map.Disable();
            rebind = option.Action.PerformInteractiveRebinding(option.Index)
                .WithControlsHavingToMatchPath(gamepad ? "<Gamepad>" : "<Keyboard>")
                .WithCancelingThrough("<Keyboard>/escape")
                .WithTimeout(10f)
                .OnCancel(operation => FinishRebind())
                .OnComplete(operation =>
                {
                    string chosen = option.Action.bindings[option.Index].effectivePath;
                    foreach (InputAction other in map.actions)
                    {
                        if (other == option.Action || (other.name == "Pause" && option.Action.name == "Restart") || (other.name == "Restart" && option.Action.name == "Pause")) continue;
                        foreach (InputBinding binding in other.bindings)
                            if (!binding.isComposite && binding.effectivePath == chosen) RebindConflict = true;
                    }
                    if (RebindConflict)
                    {
                        if (prior == null) option.Action.RemoveBindingOverride(option.Index);
                        else option.Action.ApplyBindingOverride(option.Index, prior);
                    }
                    else
                    {
                        PlayerPrefs.SetString(Preference, map.SaveBindingOverridesAsJson());
                        PlayerPrefs.Save();
                    }
                    FinishRebind();
                });
            // Keyboard/mouse rows accept mouse buttons too; movement composite
            // parts retain their Button expectation while gamepad axes retain Axis.
            if (!gamepad) rebind.WithControlsHavingToMatchPath("<Mouse>");
            rebind.Start();
            Changed?.Invoke();
        }
        private static void FinishRebind()
        {
            var completed = rebind;
            rebind = null;
            completed?.Dispose();
            map.Enable();
            suppressUntil = Time.frameCount + 1;
            Changed?.Invoke();
        }
        public static void CancelRebind() { rebind?.Cancel(); }
        public static void RestoreDefaults()
        {
            CancelRebind(); Build(); map.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey(Preference); PlayerPrefs.Save(); RebindConflict = false;
            Changed?.Invoke();
        }
        private static void Build()
        {
            if (asset != null) return;
            InputActionAsset source = Resources.Load<InputActionAsset>("Controls/Gameplay");
            if (source == null) throw new InvalidOperationException("Missing Controls/Gameplay Input Actions asset.");
            asset = UnityEngine.Object.Instantiate(source);
            map = asset.FindActionMap("Gameplay", true);
            string saved = PlayerPrefs.GetString(Preference, "");
            if (saved.Length > 0)
            {
                try { map.LoadBindingOverridesFromJson(saved); }
                catch (Exception)
                {
                    map.RemoveAllBindingOverrides(); PlayerPrefs.DeleteKey(Preference);
                    Debug.LogWarning("Saved bindings could not be loaded; using defaults.");
                }
            }
            InputSystem.onActionChange -= OnActionChange;
            InputSystem.onActionChange += OnActionChange;
            map.Enable();
        }
        private static void OnActionChange(object target, InputActionChange change)
        {
            if (change != InputActionChange.ActionPerformed || !(target is InputAction action) || action.activeControl == null) return;
            bool gamepad = action.activeControl.device is Gamepad;
            if (!gamepad && !(action.activeControl.device is Keyboard) && !(action.activeControl.device is Mouse)) return;
            if (usingGamepad == gamepad) return;
            usingGamepad = gamepad;
            Changed?.Invoke();
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            Changed = null;
            CancelRebind();
            InputSystem.onActionChange -= OnActionChange;
            if (asset != null) { asset.Disable(); UnityEngine.Object.Destroy(asset); }
            asset = null; map = null; usingGamepad = false; suppressUntil = -1; RebindConflict = false;
        }
    }
}
