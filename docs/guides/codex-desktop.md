# Run multiple Codex Desktop accounts on Windows

Codex Desktop needs both its desktop state and agent home separated. A CLI configuration profile alone is not enough.

## Desktop, not CLI

Select Codex Desktop in Hatrack. The ordinary ChatGPT desktop installation and Codex CLI are different applications. Packaged Codex can use a desktop executable named ChatGPT.exe; Hatrack checks its desktop resources and metadata.

## How isolation is configured

Each new profile gets its own CODEX_HOME, CODEX_ELECTRON_USER_DATA_PATH, and Chromium user-data directory. Profile configuration uses file-based credential storage. Credentials are not copied from the default session.

## Importing an existing home

The original .codex home cannot be adopted. Imported homes must explicitly select file credential storage. Browser state from an old launcher may not be present, so sign-in can be required again.

## Compatibility gate

Desktop override behaviour is version-dependent. This preview is not certified for independent authenticated accounts. Two simultaneous sign-ins, sign-out independence, restart persistence, and unchanged original state are mandatory release checks. WSL and managed authentication policies need separate validation.

---

[Download Hatrack](https://github.com/MuneefMumthas/hatrack/releases/latest) · [All guides](README.md) · [Back to the project](../../README.md)
