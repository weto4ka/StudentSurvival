# Menu camera repair and interface audio

Date: 2026-11-21.

## Obsolete camera component

The menu camera (`GameObject 816257227`) contained a native component of type 92 at component index 2 (`816257230`). Unity's legacy [class ID reference](https://docs.unity3d.com/ja/560/Manual/ClassIDReference.html) identifies this as GUILayer. The obsolete-GUI warning and missing-component-index warning therefore describe the same serialized component.

The repair removes that exact component and its camera component-list reference from `Assets/menu.unity`. Transform, Camera, AudioListener and FlareLayer remain intact. Type 124 is FlareLayer and is still listed in the [Unity 6 class ID reference](https://docs.unity.com/en-us/engine/6000.3/manual/working-with-scenes/text-scene-format/class-idreference); it was not indiscriminately deleted from either scene. Modern UGUI does not use GUILayer.

## Why hover was difficult to hear

The generated hover WAV had peak -19.8 dBFS and RMS -29.7 dBFS over only 65 ms. Default interface volume 0.6 and master volume 0.8 further attenuated it by approximately 6.4 dB. This is a source-level explanation for weak audibility, particularly beneath existing menu music; runtime output and the user's speakers have not been listened to or verified.

`Tools/GenerateUiSounds.py` now normalizes each completed cue to a deliberate peak with headroom for overlapping hover/click playback. Hover lasts 90 ms at 880 Hz and peaks at 0.42 (-7.5 dBFS); click peaks at 0.55 (-5.2 dBFS), and pause/resume at 0.45 (-6.9 dBFS). Four PCM WAVs were regenerated without playback. These are original nonverbal interface cues. Existing music remains in use.

`UIAudio` retains 2D playback and `ignoreListenerPause`, explicitly sets cue gain to one and priority to 16, reports missing cue resources rather than failing silently, and can recreate its persistent singleton if a later UI event finds it missing. Gain still respects both user master/interface sliders; zero volume remains mute. Root-owned hover/focus visuals are separate feedback.

Source tracing confirms `UIControlSound` already sends hover for pointer-enter/controller selection; button actions already invoke click, while non-button controls use pointer-click. Pause/resume react to the session pause state. Those paths do not need duplicate event handlers.

## Checks and user verification

The serialized Project AudioManager has audio enabled and project volume one. No application-wide or Editor mute settings were changed. The expected Windows Editor preference registry location exposed no saved audio/mute values in the read-only query; this does not establish whether the Game view's Mute Audio toggle or the operating-system mixer is enabled.

Static scene-reference checks confirmed the obsolete component/reference are absent while Camera/AudioListener/FlareLayer records remain. Audio-file analysis confirmed the generated amplitudes and zero clipped PCM samples. Whitespace checks passed. No Play Mode, build, runtime test or audio playback was started.

User checks: reload the menu scene and verify both camera warnings stop; with Master 80% and Interface 60%, move the pointer between enabled menu buttons, select them with a controller, click a button, then pause/resume gameplay. Hover, click and pause/resume should be distinct and audible. Master/Interface zero should silence cues. If all audio remains silent, report the Game view mute state and Windows application-mixer state before changing any global preferences.
