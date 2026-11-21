# Survival Games 

Implementation notes (2026-11-21): see [Docu/Start.md](Docu/Start.md), [Updates.md](Updates.md), and the [user test checklist](Docu/Modernization/User_Test_Checklist.md). Version 1.2.0 includes a guided two-exam run and Unity Localization for English, Ukrainian, Russian, French, Italian, German and Spanish. Gameplay verification is pending user testing.

This game about students adventures in the university X. Get bachelor's degree or die trying.
## Getting Started
This game build by the Unity3D engine. Version 6000.3.27f1 (Unity 6.3).
### Prerequisites
System Requirements for running Unity games :


Generally content developed with Unity can run pretty much everywhere. How well it runs is dependent on the complexity of your project. More detailed requirements:

        OS: Windows 10 version 21H1 (build 19043) or newer.
        Graphics card: DX10, DX11, DX12 or Vulkan capable GPU.
        CPU: x86 or x64 with SSE2 instruction set support, or Arm64.

### Controls
The game uses the Unity Input System package (`com.unity.inputsystem`). In Project Settings > Player > Other Settings,
set **Active Input Handling** to **Input System Package (New)**. Default bindings are editable in `Assets/Resources/Controls/Gameplay.inputactions`. Players can rebind them through How to play / Controls; `GameInput.cs` reads the asset and saves overrides.

    Move: A / D or the arrow keys (gamepad: left stick or D-pad)
    Jump: Space (gamepad: south / A)
    Crouch: hold S / Down (gamepad: down on left stick or D-pad)
    Action (open doors, pick things up): E (gamepad: west button)
    Throw a lab for one point of Sleep: Left Ctrl or left mouse button (gamepad: right trigger)
    Use the item in a toolbar slot: keys 1 - 9
    Show / hide inventory: Tab (gamepad: Select / View)
    Previous / next item: brackets (gamepad: shoulders)
    Use selected item: F (gamepad: north / Y)
    Pause: Esc (gamepad: Start); corner gear: settings (language dropdown, master/UI volume, text size, window mode)
    Occupied toolbar slots fade after use; Tab keeps the bar open. Click a slot to use it or hover for its effect.
    Editor Play starts at menu by default; Tools > Student Survival can select the open scene instead.
    After the level is won or lost: R restarts, Esc goes back to the menu

### Installing
    Step one: Download the latest version of the game.
    Step two: Open the folder "Latest Version".
    Step three: Run the .exe file.
    The last step: Select the settings in the open window.
    Congratulations!
## Authors

See also the list of contributors who participated in this project.
