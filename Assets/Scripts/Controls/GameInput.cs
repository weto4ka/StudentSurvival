using UnityEngine;
using UnityEngine.InputSystem;

namespace StudentSurvival.Controls
{
    /// <summary>
    /// The single place that reads player input, built on the Input System package.
    /// The actions are created in code, so no .inputactions asset has to be wired into any scene.
    /// Bindings mirror the old Input Manager axes (arrows/A-D, Space, Ctrl or left mouse button, E, 1-9).
    /// </summary>
    public static class GameInput
    {
        /// <summary>Number of slots in the toolbar, one key (1-9) per slot.</summary>
        public const int SlotCount = 9;

        private static InputActionMap map;
        private static InputAction move;
        private static InputAction jump;
        private static InputAction action;
        private static InputAction fire;
        private static InputAction restart;
        private static InputAction backToMenu;
        private static InputAction[] slots;

        /// <summary>Horizontal movement from -1 (left) to 1 (right).</summary>
        public static float Horizontal
        {
            get { Build(); return move.ReadValue<float>(); }
        }

        /// <summary>True only in the frame the jump button went down.</summary>
        public static bool JumpPressed
        {
            get { Build(); return jump.WasPressedThisFrame(); }
        }

        /// <summary>True only in the frame the action button (E) went down.</summary>
        public static bool ActionPressed
        {
            get { Build(); return action.WasPressedThisFrame(); }
        }

        /// <summary>True for as long as the action button (E) is held.</summary>
        public static bool ActionHeld
        {
            get { Build(); return action.IsPressed(); }
        }

        /// <summary>True only in the frame the fire button went down.</summary>
        public static bool FirePressed
        {
            get { Build(); return fire.WasPressedThisFrame(); }
        }

        public static bool PointerFirePressed
        {
            get { return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame; }
        }

        public static bool PausePressed { get { return BackToMenuPressed; } }

        /// <summary>True only in the frame the restart button went down (end-of-game screen).</summary>
        public static bool RestartPressed
        {
            get { Build(); return restart.WasPressedThisFrame(); }
        }

        /// <summary>True only in the frame the back-to-menu button went down (end-of-game screen).</summary>
        public static bool BackToMenuPressed
        {
            get { Build(); return backToMenu.WasPressedThisFrame(); }
        }

        /// <summary>Pointer position in screen pixels, zero when there is no mouse or touch screen.</summary>
        public static Vector2 PointerPosition
        {
            get
            {
                Pointer pointer = Pointer.current;
                return pointer != null ? pointer.position.ReadValue() : Vector2.zero;
            }
        }

        /// <summary>True only in the frame the key of the given toolbar slot (0-8) went down.</summary>
        public static bool SlotPressed(int index)
        {
            Build();
            return index >= 0 && index < SlotCount && slots[index].WasPressedThisFrame();
        }

        // Runs before the first scene loads. With "Enter Play Mode Options" (no domain reload) the statics of the
        // previous play session would otherwise survive, so the old actions are disposed and rebuilt on demand.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            if (map != null)
            {
                map.Disable();
                map.Dispose();
            }

            map = null;
            move = null;
            jump = null;
            action = null;
            fire = null;
            restart = null;
            backToMenu = null;
            slots = null;
        }

        private static void Build()
        {
            if (map != null) return;

            map = new InputActionMap("Gameplay");

            move = map.AddAction("Move", InputActionType.Value);
            move.expectedControlType = "Axis";
            move.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/leftArrow")
                .With("Positive", "<Keyboard>/rightArrow");
            move.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/a")
                .With("Positive", "<Keyboard>/d");
            move.AddBinding("<Gamepad>/leftStick/x");
            move.AddBinding("<Gamepad>/dpad/x");

            jump = map.AddAction("Jump", InputActionType.Button);
            jump.AddBinding("<Keyboard>/space");
            jump.AddBinding("<Gamepad>/buttonNorth");

            action = map.AddAction("Action", InputActionType.Button);
            action.AddBinding("<Keyboard>/e");
            action.AddBinding("<Gamepad>/buttonWest");

            fire = map.AddAction("Fire", InputActionType.Button);
            fire.AddBinding("<Keyboard>/leftCtrl");
            fire.AddBinding("<Mouse>/leftButton");
            fire.AddBinding("<Gamepad>/buttonSouth");

            restart = map.AddAction("Restart", InputActionType.Button);
            restart.AddBinding("<Keyboard>/r");
            restart.AddBinding("<Gamepad>/start");

            backToMenu = map.AddAction("BackToMenu", InputActionType.Button);
            backToMenu.AddBinding("<Keyboard>/escape");
            backToMenu.AddBinding("<Gamepad>/select");

            slots = new InputAction[SlotCount];

            for (int i = 0; i < SlotCount; i++)
            {
                slots[i] = map.AddAction("Slot" + (i + 1), InputActionType.Button);
                slots[i].AddBinding("<Keyboard>/" + (i + 1));
                slots[i].AddBinding("<Keyboard>/numpad" + (i + 1));
            }

            map.Enable();
        }
    }
}
