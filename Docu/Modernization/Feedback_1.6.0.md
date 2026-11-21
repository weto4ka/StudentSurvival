# Readability, anatomy and combat feedback — 1.6.0

Date: 2026-11-22. Status: implementation complete; source checks passed and user runtime verification pending. No Unity runtime result is claimed. The user authorized Play Mode through MCP if available, otherwise chose to test themselves; Unity MCP commands remain unavailable in this chat.

## User report and preserved evidence

The user requests a pixel back arrow, reports illegible/dense help/HUD text, English action names mixed with German keyboard names, unnatural crouching and visibly detached jumping legs, slow/confusing F behavior, an unexplained long yellow line at the teacher, and unclear lab location/resource use. Screenshots are preserved without edits under Attachments/1.6.0: [help](Attachments/1.6.0/help.png), [controls](Attachments/1.6.0/controls.png), [crouch](Attachments/1.6.0/crouch.png), [jump](Attachments/1.6.0/jump.png), [HUD](Attachments/1.6.0/hud.png).

These were substantive implementation problems. The yellow lane and speculative articulated poses were modernization additions, not original jam behavior. Prior source compilation did not establish that they looked natural or were understandable in-game.

## Readability and UI

- Canvas reference changes from1600×900 to1280×720. The previous small body font was scaled down heavily in the user's small Game view. Help/credits now choose pixel-grid font sizes from actual screen scale (body16–32, heading24–48) aiming for readable physical text, use1.5 line spacing, wider section gaps, no small-body best-fit shrink and no body shadow. Font/layout recalculates when the view changes.
- Stats are two explicit short rows with enough height; exam progress and nearby throw hints are shorter. The duplicate objective line is removed rather than drawn over route guidance. Save status, resource state, feedback and item hints occupy separated regions. Action warnings suppress route text while active. Critical warning text is simply localized JUMP/CROUCH plus the actual binding.
- Back arrows are drawn as square pixel cells, without diagonal thin strokes. The same pixel arrow marks the nearest available lab during first-minute guidance. Its screen position tracks the cached world target and camera every frame; route/target choice updates every0.4s from cached pickup transforms.
- Keyboard binding labels are derived from effective paths and canonical Input System layout metadata for named keys. Printed single-letter/digit glyphs still follow the actual keyboard layout (QWERTZ/AZERTY); numpad names stay distinct. English menus therefore no longer inherit German Strg/Leer/Nach-Links names. This is a neutral physical-key label convention, not a claim of fully localized controller-brand glyphs. Rebind/device changes refresh guide and hint labels.

## Natural poses and collider consistency

CrouchVisual uses the original pixel Idle/1.psd, measured waist/hip/knee/ankle anchors and actual parent-child transforms. Head, torso, arms and pelvis remain one intact crop; thighs/shins/shoes overlap at joints. Crouch has15° torso lean and both knees forward with independently flat shoes, replacing the55° fold/opposing leg splay. Ground height is solved from shoe soles. Jump/landing/standing recovery share this rig, tint and sorting group so body parts stay together and are not sorted behind scenery.

Main explicitly serializes Character.crouchHeight=.75, as well as the changed script default, to avoid an existing Editor scene retaining the old value. Crouch collider height changes from55% to75% of standing height to match a plausible unsquashed body. Visible crouch top is about102.91/140 source pixels, approximately2.10 world units including the sprite offset. The2.808-unit standing collider becomes2.106 units when crouched. The examiner's high throw moves to2.55 units above its feet; with radius0.22, its lower edge2.33 clears this crouch and still intersects a standing student. Low throws remain0.8.

Offline mechanical composition previews of the unchanged source pixels are in .utmp/pose-diagnostics (ignored staging): standing/crouch/rising/falling/landing plus12 transition samples. Root viewed crouch and rising, identified a blue overhang, and had the shin crop reduced from8px to2px above the knee. This removed the protrusion while retaining6px knee overlap. Preview connectivity and visible silhouette were both inspected; neither is a substitute for Unity animation/collision verification.

## What F does and where to find labs

F/Y uses the selected inventory item; it is not unlimited ammunition. Collect authored reports on the ground to the right of the starting position with E/X. New pickups select their slot automatically. A visible pixel marker and explicit pickup prompt identify lab ammunition. Brackets/LB/RB change selection; inventory now explains exactly what F will use on keyboard too.

A press uses immediately. Holding F/Y repeats only selected lab reports every0.18s (about5.6 per second), spending one report per throw without Sleep cost. Holding on food/drinks/books never repeatedly consumes them. Empty slots and depleted stacks give explicit feedback. There is no silent paid-throw fallback when a lab stack empties.

Ctrl/LMB/RT throws fresh reports, costing1 Sleep per projectile; holding now repeats at the same0.18s cadence. Zero-Sleep paid shots remain blocked; collected ammo stays usable during rescue. Pointer holds that began on UI/while paused require release before firing into the world, avoiding paid shots from holding a menu/inventory click. Projectile speed remains10; a distant projectile still takes travel time. The reported delay had no existing cooldown in source: old F was press-only, plus a finite/possibly non-lab selection.

## Teacher warning

The long14-unit yellow/cyan world line is removed. A compact above-head wind-up cue and subtle teacher pulse precede the existing attack timing; HUD text tells the player which dodge/binding to use. The guide explains that these marks mean an incoming grade. Warning timings and the final alternating two-shot pattern remain; no laser-like lane must be interpreted.

## User verification focus

Stop Play Mode, refresh/compile, then test version1.6.0. Check help/control/HUD readability at the small Game view shown in the screenshots and at1280×720/1920×1080. Confirm canonical key names, square back arrows, actual lab marker/collection/auto-selection, F hold and finite ammo, no repeat consumption of food, and no paid shot from a held UI click. Inspect crouch both ways, headroom, jump/apex/landing connections, high-throw clearance and compact teacher warnings. Native wording, feel, appearance and actual runtime performance remain user checks.

## Learnings

- A single connected opacity component does not prove an anatomically plausible pose. Inspect actual previews and intermediate motion, not just connectivity.
- Overlap beyond a rotating knee can form a visible blade. Above-knee crop extension was8px;2px preserves seam coverage without that protrusion.
- Readable pixel typography depends on final physical scale and line spacing, not only whether a pixel font is installed.
- Input System live display names can follow OS language independently of game localization. Canonical layout names prevent that leakage.
- Explain selected-item use, resource count and source of ammo before adding repeat fire. Never make F silently spend Sleep after a free stack runs out.
