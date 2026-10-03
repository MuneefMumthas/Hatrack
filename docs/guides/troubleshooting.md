# Desktop detection and launch troubleshooting

Fix a missing location or a failed launch without resetting valuable profile data.

## The installed app is not detected

Choose Locate desktop app, select Claude or Codex, and browse to its official desktop executable. Hatrack rejects CLI tools and profile-launcher stubs. Package discovery is preferable for WindowsApps because Windows restricts folder browsing.

## Another program is currently using this file

Windows app-package updates or stale runtime state can prevent Claude from launching. Save your work, finish vendor updates, and restart Windows if the error persists. Hatrack does not kill unrelated services or reset authentication data to work around this upstream failure.

## A shortcut name already exists

Choose another name. Hatrack refuses to overwrite an unrelated shortcut and rolls back incomplete creation. Renaming a managed profile keeps its permanent identity and data folder.

## Codex rejects the credential store

An imported Codex home must set cli_auth_credentials_store = "file" at the top level of config.toml. Hatrack does not rewrite imported configuration. Organisation policies may prevent this change; do not bypass them.

## Reporting an issue

Include Windows, Hatrack, and vendor desktop versions and reproducible steps. Use the local diagnostics export, which omits credentials and profile paths. Never attach auth.json, cookies, private conversations, or account tokens.

---

[Download Hatrack](https://github.com/MuneefMumthas/hatrack/releases/latest) · [All guides](README.md) · [Back to the project](../../README.md)
