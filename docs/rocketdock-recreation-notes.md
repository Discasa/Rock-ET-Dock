# RocketDock Recreation Notes

These notes document the clean-room research used to shape Rock ET Dock. The original RocketDock installer and extracted files were treated as reference material only. Rock ET Dock uses new code, project-owned assets, and a new configuration model.

## Scope

The goal is to recreate the user-facing behavior of a classic Windows dock:

- A transparent, borderless launcher anchored to a screen edge.
- File, folder, URL, separator, Recycle Bin, Settings, and Exit items.
- Hover magnification with neighboring icons moving aside.
- Drag-and-drop for adding, reordering, removing, and exporting items.
- Auto-hide, topmost control, and screen-edge positioning.
- Theme/skin-inspired shell and tile appearance.
- Per-user settings and managed item storage.

## Legal And Distribution Boundary

- Do not ship RocketDock binaries, DLLs, skins, icons, or bundled assets unless a future license review explicitly allows it.
- Do not decompile or reuse RocketDock implementation code.
- Use the extracted installer only to understand behavior, file layout, configuration formats, and public integration points.
- Keep Rock ET Dock runtime assets project-owned.

## Reference Installer Findings

The reference artifact was an Inno Setup installer. Extracting it with `innoextract` provided useful behavior and format evidence without executing the installer.

High-value extracted areas:

| Area | Use |
| --- | --- |
| Main executable and DLLs | API and integration hints through strings and imports |
| `Defaults.ini` | Default item and behavior model |
| `Skins` | Background and separator configuration conventions |
| `Languages` | Translation-key style and feature labels |
| Help/changelog files | Feature list and expected user workflows |

The extracted material confirmed that a faithful recreation should focus on behavior and local configuration, not binary compatibility in the first version.

## Behavior Model

Core item types:

- File
- Folder
- URL/link
- Separator
- Recycle Bin
- Dock settings
- Exit
- Animated GIF
- Runtime window item

Expected interactions:

- Left-click launches or activates the item.
- Right-click opens a context menu where appropriate.
- File/folder drops add items to the dock.
- Drops onto Recycle Bin delete through native shell behavior.
- Dragging within the dock reorders items.
- Dragging an item outside the dock exports or moves it through real shell drag/drop semantics.
- Hover magnification expands the focused icon and moves neighbors aside.

## Layout Requirements

Supported edges:

- Top
- Bottom
- Left
- Right

Left and right docks must be vertical docks: the dock shell is taller than it is wide, icons remain upright, and hover/reorder motion runs along the vertical axis.

Each dock needs:

- Monitor selection.
- Edge selection.
- Edge offset.
- Center offset.
- Optional fixed width and height.
- Topmost/normal/behind layering.
- Auto-hide and mouseover popup behavior.

The geometry code should be deterministic and testable outside WPF. Rock ET Dock keeps these rules in `DockGeometry` and covers them through `tests/Dock.GeometryChecks`.

## Storage Contract

Rock ET Dock is intentionally per-user:

- Configuration: `%LOCALAPPDATA%\Rock ET Dock\dock.config.json`
- Logs: `%LOCALAPPDATA%\Rock ET Dock\logs\runtime.log`
- Managed dock root: `%USERPROFILE%\Rock ET Dock`
- Managed folder per dock: `%USERPROFILE%\Rock ET Dock\<dock-name>`

Dropped files are not loosely referenced by default. The app either moves the item into the managed dock folder or creates a shortcut there, depending on the selected import mode.

## Shell Integration

Shell-backed items must stay separate from normal filesystem items:

- The Windows button uses dedicated native menu services.
- Recycle Bin uses native shell APIs and keeps `shell:RecycleBinFolder` semantics.
- Recycle Bin context menus should be native, not recreated manually.
- Special items should be excluded from generic file move/export logic.

Explorer folder drops require real OLE drag/drop behavior. A desktop-only export path is not enough because users expect Explorer windows to accept moved items.

## Settings Model

Settings should apply immediately. The UI should not depend on a manual Apply button.

Current categories:

- General: name, language, startup, locking, auto-hide, Windows button, Recycle Bin, taskbar hiding, import mode, and managed folder.
- Icons: size, opacity, labels, spacing, bottom margin, quality, zoom, hover range, and animated GIF items.
- Position: monitor, edge, layering, width, height, edge distance, and center offset.
- Style: theme, background opacity, shell radius, tile radius, font, font size, and label color.
- Behavior: minimized-window items, running indicators, existing-instance activation, and mouseover popup.

Language switching should be centralized. Rock ET Dock uses `TextCatalog` for English and Brazilian Portuguese instead of scattering hardcoded UI labels through windows and menus.

## Theme Direction

RocketDock skin names are useful as reference cues, but Rock ET Dock should not reuse original skin files. The current implementation maps known skin names to project-owned color palettes and shape values.

Future theme work can add:

- Import support for compatible background/separator metadata.
- 9-slice or stretchable bitmap rendering for user-supplied skins.
- A clear license boundary for any community skin packs.

## Runtime Features Still Worth Exploring

Potential future work:

- DWM thumbnails for minimized windows.
- A safer plugin model inspired by docklets, without loading legacy DLLs directly.
- Theme import/export.
- Per-dock backup and restore.
- More precise process/window matching for complex launch targets.

## Implementation Guidance

- Prefer WPF and .NET services over compatibility shims.
- Keep Win32 and COM-heavy shell behavior behind focused service classes.
- Keep persistent models simple and JSON-based.
- Keep layout math isolated and covered by executable checks.
- Use user-profile storage by default.
- Treat the old installer as reference-only material.
