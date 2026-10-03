# Install and create desktop profiles

Download `Hatrack-Setup.exe` from Releases. Run it, accept the licence, and open Hatrack. It installs for your Windows user, without developer dependencies.

## Locate the official app

Hatrack checks registered Windows packages and standard locations. If it cannot find an app, choose **Locate desktop app**, select Claude or Codex, then choose its desktop executable. The selector checks vendor/product metadata and desktop resources; a CLI executable is rejected.

For packaged Codex Desktop the executable can be named `ChatGPT.exe` inside the **OpenAI.Codex** package. The ordinary ChatGPT desktop package and Codex CLI are different applications and are rejected. WindowsApps can restrict manual browsing; automatic package discovery is preferable.

If the app is absent, use **Official download** in the creation flow, install it from the vendor, and leave Hatrack open. Detection is retried automatically.

## Create and recognise profiles

Choose **Create profile**, select an app, enter a distinct name, and choose an original icon design or upload an image. Uploaded images must be PNG, JPEG, or ICO, at most 10 MB and 4096 pixels per side. Adjust padding, crop, colour, and background. The preview shows Desktop and taskbar sizes.

Create shortcuts, then open the profile and sign in within the official app. Profile logins are not copied from your original session. Pin its running taskbar button manually through Windows. After changing an icon, reopen that profile; pinned shortcuts may need unpinning and repinning.

## Import carefully

Discovered folders are candidates. Select the right one or browse to a specific existing data folder. Hatrack creates its own shortcuts while retaining existing shortcuts. Duplicate folders cannot be imported twice. The original Codex home cannot be imported.

Imported Codex homes must use `cli_auth_credentials_store = "file"` at the top level of their `config.toml`. Hatrack will not overwrite an imported configuration. Agent homes alone may not contain the old desktop browser state, so signing in again may be necessary.

## Removal and recovery

**Remove launcher** removes the library entry and owned shortcuts. It preserves account data. Uninstall does the same for this installation's shortcuts and preserves the catalogue and all data. Reinstall, edit a profile, and save to recreate its shortcuts.

Never delete your entire `.codex`, AppData, or WindowsApps folders to fix a launch failure. Finish vendor updates and restart Windows if it reports a locked app executable. This can be an upstream Windows package issue. No unrelated processes or services are killed by Hatrack.

A corrupt catalogue fails closed. After closing Hatrack, preserve the damaged file, inspect `catalog.json.bak`, and restore a known-good catalogue. Profile data is separate from the catalogue. Do not share catalogue backups or authentication files in issue reports.
