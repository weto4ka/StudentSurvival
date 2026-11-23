# Restored open WC entrance

Date: 2026-11-22. Implementation version: 1.8.1.

The user preferred the earlier recessed, open WC entrance to the replacement wooden door. PracticalClassroom now uses that exact earlier artwork, including its WC plaque, through the WCOpenEntrance sprite slice in CampusSignage.png. The separate overlaid plaque is inactive to avoid a duplicate sign. The original source image pixels are unchanged; the crop and placement remain editable in Unity.

The open entrance is3.05 units high and approximately2.05 units wide, centered on the existing WC interaction at local X−3.4. The existing Interact trigger, room ID4 destination, separate toilet room, unlock rule and clear return arrival are retained. Other classroom doors are unchanged.

Offline checks pass for sprite crop bounds, unique prefab IDs, sprite references, and retained destination/arrival references. No source logic changed, and no computer control or gameplay test was performed. The1.8.0 import/static reports describe the preceding layout; they are not a new visual verification of this artwork change.

User check: outside Play Mode inspect the practical room's WC entrance; expect the dark open recess and original WC sign, without a wooden door leaf or a second plaque. In Play Mode use the effective Interact binding there; expect travel to the separate WC, then return beside the same entrance. Review any existing prefab-instance artwork overrides if the old door remains visible.
