# Intro timestamps and passage markers

2026-11-22 · Version 1.8.3

The Windows Unity player reported unexpected timestamps on usb_soft.mp4. The input was H.264 Main with two reordered B-frames and a non-picture packet. FFmpeg decoded 97 pictures from 98 reported packets. The replacement keeps all 97 pictures at 30000/1001 fps with constant, zero-based presentation and decode timestamps. H.264 Constrained Baseline, BT.709 tags, AAC packet hash and the Unity asset GUID were checked. Full FFmpeg decoding passed. Unity playback remains pending.

The user preferred a slim line beside the passage. Five campaign portals now use a narrow sprite at the right side of their trigger with the existing status indicator above it. The open WC entrance artwork was kept. Comparing prefab data confirmed trigger and destination components were unchanged.

To check in Unity, replay the intro and look for the timestamp warning, then approach the first classroom passage and use Interact after passing the exam.

## Passage follow-up · 2026-11-22 · 1.8.4

Left exits now have markers against the left wall and right exits have markers against the right wall. Exit trigger boxes extend toward each wall with the room-facing boundary preserved. The separate toilet has a left-edge return marker. The user clarified that the practical-room WC entry should keep its open recess and integrated WC plaque. Static geometry checks confirm that the outside edge of each of the five exit lines meets its room boundary. Serialized portal destinations and arrival settings are unchanged.
