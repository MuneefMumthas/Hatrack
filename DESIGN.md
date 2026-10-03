---
name: Hatrack
description: Precise monochrome desktop controls with colourable profile marks.
colors:
  canvas: "#F5F5F7"
  surface: "#FFFFFF"
  inset: "#EEEEF0"
  ink: "#161617"
  muted: "#626267"
  line: "#C7C7CD"
  hover: "#E7E7EB"
  selection: "#DCDCE1"
  error: "#B42318"
  dark-canvas: "#111113"
  dark-surface: "#1E1E21"
  dark-inset: "#27272B"
  dark-ink: "#F5F5F7"
  dark-muted: "#B5B5BD"
  dark-line: "#65656E"
  dark-hover: "#303035"
  dark-selection: "#3D3D44"
  dark-error: "#FFB4AB"
  web-canvas: "#FAFAFA"
  web-line: "#D8D8DE"
  web-inset: "#EFEFF2"
  web-dark-line: "#45454C"
  preview-light: "#F3F3F5"
  preview-dark: "#252528"
  claude-default: "#D97757"
  codex-default: "#7C3AED"
typography:
  native-body:
    fontFamily: "Segoe UI"
    fontSize: "14px"
  native-heading:
    fontFamily: "Segoe UI"
    fontSize: "28px"
    fontWeight: 600
  native-profile-name:
    fontFamily: "Segoe UI"
    fontSize: "17px"
    fontWeight: 600
  native-caption:
    fontFamily: "Segoe UI"
    fontSize: "12px"
  web-display:
    fontFamily: "DM Sans Variable, sans-serif"
    fontSize: "clamp(56px, 6.5vw, 90px)"
    fontWeight: 650
    lineHeight: 1.03
    letterSpacing: "-0.04em"
  web-heading:
    fontFamily: "DM Sans Variable, sans-serif"
    fontSize: "clamp(30px, 3.5vw, 48px)"
    fontWeight: 600
    lineHeight: 1.12
    letterSpacing: "-0.035em"
  web-lead:
    fontFamily: "DM Sans Variable, sans-serif"
    fontSize: "19px"
    lineHeight: 1.65
  web-body:
    fontFamily: "DM Sans Variable, sans-serif"
    fontSize: "16px"
    lineHeight: 1.7
rounded:
  checkbox: "6px"
  menu-item: "8px"
  control: "11px"
  dropdown: "12px"
  preview: "14px"
  panel: "16px"
  web-image-frame: "24px"
  web-stage: "28px"
  web-button: "30px"
spacing:
  control-gap: "8px"
  row-gap: "12px"
  checkbox-gap: "16px"
  panel-inset: "18px"
  rail-inset: "24px"
  editor-inset: "28px"
  web-gutter: "48px"
components:
  native-button:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.ink}"
    rounded: "{rounded.control}"
    padding: "10px 16px"
  native-primary:
    backgroundColor: "{colors.ink}"
    textColor: "{colors.surface}"
    rounded: "{rounded.control}"
    padding: "10px 16px"
  native-primary-dark:
    backgroundColor: "{colors.dark-ink}"
    textColor: "{colors.ink}"
    rounded: "{rounded.control}"
  native-input:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.ink}"
    rounded: "{rounded.control}"
    padding: "11px 13px"
  native-profile-row:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.ink}"
    rounded: "{rounded.panel}"
    padding: "18px"
  web-button:
    backgroundColor: "{colors.ink}"
    textColor: "{colors.surface}"
    rounded: "{rounded.web-button}"
    padding: "17px 25px"
  web-note:
    backgroundColor: "{colors.web-inset}"
    textColor: "{colors.ink}"
    rounded: "{rounded.panel}"
    padding: "24px 28px"
---

# Design System: Hatrack

> The website was removed in 0.2.0. Website tokens and sections below are kept as a record and no longer ship.

## Overview

The approved direction is monochrome, rounded, and precise, with the requested craft references of Apple and Revolut Wallet. The desktop implementation uses a shared custom WPF theme for buttons, dropdowns, inputs, sliders, checkboxes, expanders, list focus, and scrollbars. Practical headings and task labels establish the hierarchy. Colour belongs to profile marks and their picker, while product chrome remains black, white, and neutral grey.

Hatrack retains its two-shelf hat rack mark in monochrome. Claude and Codex marks are exact source silhouettes recoloured through opacity masks for profile icons. These vendor marks retain their respective rights and are not original MIT artwork. The application remains independently branded.

**Key Characteristics:**

- Monochrome surfaces with distinct light and dark text contrast.
- Custom rounded controls with visible keyboard and selection states.
- Recolourable app marks and preserved legacy profile icons.
- Practical copy and explicit preview status.

## Colors

Frontmatter records the implemented colours. Native theme roles are `Canvas`, `Surface`, `Inset`, `Ink`, `Muted`, `Line`, `Hover`, `Accent`, `OnAccent`, `Selection`, and `Error`. In light mode Accent matches Ink and OnAccent matches Surface. In dark mode Accent matches dark Ink and OnAccent uses the light-mode Ink colour, producing near-white primary buttons with dark text. The owner theme is shared by editor, picker, and attached dialogs. System appearance reads the Windows application-theme preference when applied, and window chrome receives dark-mode and rounded-corner attributes.

The website has separate canvas, border, and inset values where specified. Dark website colours otherwise match native dark roles, except the quieter website border. CSS custom properties remain the website source of truth. Appearance starts from saved local storage or system preference; the toggle persists the selected light/dark choice and updates its accessible label, pressed state, and browser theme colour.

Profile defaults are terracotta for Claude and violet for Codex. The eight preset swatches are black, white, blue, violet, yellow, red, orange, and pink. A custom RGB picker supplies any opaque hex colour. Icon background choices are Transparent, white, and near-black. The large icon inspection tile remains white in both themes; the two taskbar tiles retain fixed light and dark grounds.

## Typography

Native screens use Segoe UI at 14 WPF device-independent units. The library title is 28 semibold, editor title 26 semibold, wordmark/empty heading/picker title 24 semibold, preview heading 18 semibold, profile names 17 semibold, and metadata/footer status 12. Buttons use Medium weight. Labels use semibold. Native line heights follow text defaults where not explicitly set.

The website self-hosts DM Sans Variable through `@fontsource-variable/dm-sans`, with a sans-serif fallback. Its display and heading scales are in frontmatter, with balanced wrapping and tight tracking. The wordmark is 24 at weight 650. Feature titles are 21 at weight 600. Supporting paragraphs use 16px at line-height 1.7, lead copy 19px at 1.65, and captions 12px. Lead text is capped at 65 characters, with the hero limited to 600px.

Articles use a fluid 40 to 64px title, 28px section headings, and 17px text at line-height 1.8. Code uses browser monospace. At the mobile breakpoint, hero titles are 52px, article titles 42px, leads 17px, and article body text 16px. Copy names concrete actions and avoids vague slogans and em dashes.

## Layout

The library opens at 1120 by 760, with a 920 by 640 minimum. Its 230-unit rail has a 24-unit inset and right divider. The main area has 36-unit side margins, top 32, and bottom 24. Title/action, app detection, search/filter, scrolling profile rows, and status occupy separate grid rows. The filter column is 180 units. Each profile row places a 40-unit mark in a 56-unit column, name and metadata in the flexible centre, and Open/Edit on the right. Long names truncate with an ellipsis and retain a full-name tooltip.

The editor opens at 840 by 780, with a 760 by 650 minimum and 28-unit inset. Fields scroll vertically beside a fixed 240-unit preview column. Save/Cancel remain docked below. Shortcut and Favourite controls share a wrap row outside the advanced expander, keeping them available when advanced controls collapse. The large preview is 160 square; both taskbar previews are 32 square on padded inspection tiles. The RGB picker is a fixed 420 by 440 window with 28-unit inset, 64-unit colour tile, three labelled channel rows, hex output, and right-aligned actions.

Website content and header are capped at 1280px with 48px side gutters. The hero is centred, with a 900px copy column and a 1080px screenshot stage. The stage has 22px padding and sits 56px below the actions. A secondary screenshot frame uses 28px padding and caps its image at 800px. Features have two equal columns with a 100px gap; setup steps have three columns with 48px gaps. Articles use an 830px column with 32px inner gutters.

At 1000px, feature gaps and closing type shrink. At 700px, side gutters become 24px, features/setup become single columns, the footer wraps, and screenshot frames reduce padding and corner sizes. The hero remains centred with a smaller title. Header Guides hides, Source becomes its accessible icon, and download/theme actions remain. The native utility supports resizing and vertical scrolling rather than mobile layouts.

## Elevation & Depth

Surfaces use tonal layering, borders, and spacing. The final implementation has no custom shadows or glows. The website displays actual light/dark screenshots selected by theme. Its screenshot stage arrives once over 0.65 seconds, moving 16px and removing a 2px blur. Website button hover changes opacity over 0.18 seconds. Reduced-motion preference removes these effects and smooth scrolling. Native dropdown popups use a fade; button state opacity changes are immediate.

## Shapes

Native control corners are 11 units, checkbox corners 6, dropdown rows 8, dropdown popup 12, large previews 14, and library panels/profile rows 16. The scrollbar is 10 units wide with a rounded thumb. Website actions are pill-like at 30px radius. Screenshot stage/frame corners are 28/24px and images 14px; mobile frames reduce to 16px and images 10px. Website appearance toggle is circular at 40px square.

## Components

### Buttons and navigation

Native buttons have 42-unit minimum height, 16/10 insets, and a one-unit border. Hover uses 0.78 opacity and pressed uses 0.6. Keyboard focus changes the border to Ink at two units; disabled controls use 0.55 opacity and an arrow cursor. Primary buttons invert fill/text between themes. Editor Save has 22/11 insets. Navigation buttons are transparent; the selected All profiles/Favourites row has Hover fill and semibold text.

Website download links use inverse monochrome fill/text with a 0.8 hover opacity. Standard buttons have 17px/25px padding; header actions use compact padding. Keyboard focus uses a two-pixel Ink outline with five-pixel offset. Ordinary links thicken underlines on hover. The skip link moves into view when focused.

### Fields and dropdowns

TextBox controls use 11-unit corners, Ink caret/text/selection text, Selection highlight, Surface fill, and Line border. The shared minimum is 44; editor controls explicitly set 40. Focus produces a two-unit Ink border. Dropdowns use matching rounded fields, custom chevrons, a six-unit popup gap, five-unit popup inset, and a scrollable list capped at 320 units. Highlighted rows use Hover; selected rows use Selection and semibold text. Disabled opacity is 0.55. Accessible names identify search and editor fields.

### Checkboxes, sliders, and advanced controls

Checkboxes use 20-unit squares with a monochrome check, six-unit corners, a 24-unit icon column, and a 10-unit label gap. Checked fill is Accent; keyboard focus adds a two-unit Ink border. Slider tracks are four units high, with Ink progress, Line remainder, and a Surface-filled 19-unit circular thumb outlined in Ink. Keyboard focus enlarges the thumb to 23. The custom advanced expander uses Inset fill, 11-unit corners, a chevron, and collapsible content beneath an eight-unit gap.

### Editor, profile marks, and recovery

New profile app selection defaults to its matching Claude/Codex mark and colour. Recolouring preserves the source silhouette; mark rendering starts at 208 units within a 256-unit icon canvas before zoom/crop. Existing legacy shapes are added to the selector when editing and retain their renderer. Uploaded artwork is preserved and can be replaced by Use design. Upload supports PNG/JPEG/ICO with a 10MB and 4096px-per-side limit. Advanced controls adjust size, crop, and background.

Detection hides Locate app/Official download once the selected desktop app is found. Missing detection reveals these actions and disables Save. Existing/imported profiles lock app selection. The RGB picker has integer Red/Green/Blue sliders from 0 to 255, numeric output, live colour tile, hex output, Cancel, and Use colour. Large and both taskbar previews share the rendered image to expose contrast on light and dark grounds.

Validation wraps above editor actions using Error. The library offers first-profile and no-match recovery messages. Status and warning dialogs report failures. Removing a launcher confirms preservation of profile folders; import explains candidate evidence and preserves original shortcuts. Preview labels remain visible. Visual review disposition is ship, while independent authenticated-session acceptance remains a separate release gate.

## Do's and Don'ts

### Do:

- Do preserve the original monochrome Hatrack product mark.
- Do use colour for profile marks and picker output.
- Do retain exact vendor silhouettes and identify their separate rights.
- Do preserve legacy profile artwork when editing.
- Do retain readable contrast, visible control states, recovery actions, and preview labels.
- Do keep screenshot evidence in `docs/images` and `artifacts/visual`.

### Don't:

- Don't add glows, decorative shadows, em dashes, or vague slogans.
- Don't treat vendor source marks as original MIT artwork.
- Don't replace the custom control system with default platform templates.
- Don't present demonstration profiles or visual review as authenticated-session acceptance.
- Don't publish stable-release claims before the acceptance gates are evidenced.

Sources: `src/Hatrack/Theme.xaml`, `Theme.cs`, `MainWindow.xaml`, `EditorWindow.cs`, `ColourPickerWindow.cs`, `Icons.cs`, website CSS/Layout, and current `docs/images` desktop/editor/picker captures. The sidecar extends tokens with state, motion, breakpoint, and component examples; HTML samples illustrate WPF styling without replacing the native templates.
