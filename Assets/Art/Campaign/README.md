# Campaign artwork provenance and editing

Date: 2026-11-22. Delivery: 1.8.1.

These imagegen-assisted raster assets were created for the StudentSurvival modernization using the user's supplied original classroom screenshot as a style reference. They are additions to the project, not original game-jam artwork. Original team credits and character sprites remain attributed to the original project; this document does not settle existing third-party asset/music licensing.

| File | Use |
| --- | --- |
| PracticalRoom.png | Unique practical-room wall; the draft WC alcove was removed because the toilet is a separate room |
| OralExamRoom.png | Unique wood-paneled oral-exam wall with dusk window and certificates |
| CampusToilet.png | Separate WC's tiled wall and pipework |
| UniversityProps.png | Study computer/workbook desk, bookcase, door, chair, examiner desk/lamp, toilet, locker, archive cabinet |
| BathroomProps.png | Separately editable washbasin and wall mirror |
| CampusSignage.png | Generated draft supplying the WC plaque and restored open WC entrance sprite crops |
| SpriteLayout.json | Initial university-prop slicing coordinates; Unity's importer metadata is authoritative for edited slices |

Generated source pixels were copied into the project. Cropping/slicing and feet alignment are defined non-destructively in the Unity .png.meta importer records. Textures use Point filtering, no mipmaps and uncompressed sprite import. PNGs are not regenerated in Play Mode or on import. Each prop is an individual SpriteRenderer in an authored prefab, and can be moved, scaled or replaced in Unity. Frame/lighting architecture is painted into each wall background; the furniture, doors, washbasin and mirror are separate objects.

The approved original classroom reference is preserved at Docu/Modernization/Attachments/1.7.3/repeated-room-door-blocks.png. The later teacher and student sprites reference the original Assets/Graphic/other_characters files. Their differing pixels-per-unit settings are respected when aligning visible feet/height.

AI generation can introduce pixel-edge/color inconsistencies. Final style, framing, alpha-edge and in-game readability review is assigned to the user; no claim of a final Unity visual pass is made. Authored prefabs, not an external generator, are the maintained room source.
