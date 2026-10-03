# Changelog

## 0.2.0 preview

- Renamed from Profiles to **Hatrack**, with a new monochrome hat-rack logo, app icon, and social cards.
- Upgrades from Profiles 0.1.x keep working: the existing `%LOCALAPPDATA%\Profiles` catalogue is reused, pinned taskbar identities are unchanged, and the installer repairs profile shortcuts to point at `Hatrack.exe`.
- Untested Claude or Codex Desktop versions now ask for confirmation once per version instead of blocking launch.
- The library and editor no longer start PowerShell every few seconds once desktop apps are detected.
- The version shown in the app and diagnostics now comes from the build instead of a hard-coded string.
- Removed the website. Its guides now live in `docs/guides`, and the README carries the FAQ and search-focused copy.
- Three new regression checks (18 total).

## 0.1.1 preview

- Exact Claude and Codex app logo presets with silhouette-preserving recolouring.
- Monochrome product branding, light/dark themes, and readable control text.
- Custom rounded dropdowns, buttons, inputs, sliders, checkboxes, scrollbars, expandable image controls, and RGB colour picker.
- Compact editor with taskbar previews against light and dark backgrounds.
- Responsive light/dark website with current screenshots and direct product copy.
- Two additional regression checks for logo recolouring and dark popup foregrounds.


## 0.1.0 | Preview

- Original Profiles identity and geometric icon studio.
- Claude and Codex desktop installation detection, with manual fallback.
- Transactional profile creation and individual Windows shortcut identities.
- Search, favourites, themes, importing, and data-preserving removal.
- Self-contained Windows installer, static launch website, and release documentation.
- Isolated lifecycle checks and explicit stable-release acceptance gate.

Authenticated desktop isolation and full fresh-machine acceptance remain required before stable publication.
