# Local preview verification | 3 October 2026

This report describes observed results, not a stable compatibility certification. The current installer is **unsigned**. `compatibility.json` keeps `releaseReady: false` and blocks stable release automation.

## Environment and checks

| Check | Observed result |
| --- | --- |
| Host | Windows 11 x64, build 26200 |
| Build | .NET SDK 10.0.401, self-contained Windows x64 WPF application; Inno Setup 6.5.4 |
| Engine checks | 16 passed: validation, unsupported-version protection, invalid artwork, shortcut identity, collisions, rollback, locked metadata, immutable IDs, spaced paths, Codex local configuration, removal, concurrency, corrupt catalogue |
| Claude Desktop | Package 2.19675.0.0: two simultaneous isolated test windows, distinct profile AppUserModelIDs, separate populated user-data directories |
| Codex Desktop | Package 26.930.2377.0: two simultaneous isolated test windows, distinct profile AppUserModelIDs, separate populated agent and desktop directories |
| Installer | Isolated install, reinstall, installed executable launch, and uninstall passed; catalogue and data sentinel preserved |
| Website | Eight static pages built; local links and metadata passed; Chromium checks at 1440 × 1000 and 390 × 844 found no horizontal overflow, broken images, or page errors |
| UI review | Monochrome theme, exact app-logo silhouette recolouring, dark input/dropdown text, custom controls, and responsive website checked |
| Artwork | Original monochrome product logo and social graphic, plus colourable vendor app marks; demonstration screenshots from the actual WPF interface |

Installer testing uses a separate test registration, installation folder, and profile home. Desktop smoke testing creates disposable profiles without adopting existing data, does not sign into accounts, and closes only the test process trees. Runtime data is excluded from the source project and repository.

## Reproduce

Use the .NET 10 SDK and Inno Setup 6 as documented in the README:

```powershell
./scripts/build.ps1
./scripts/test-installer.ps1
```

For desktop smoke checks on the recorded vendor versions, use a **new** folder whose name begins `Hatrack-smoke-`:

```powershell
./artifacts/publish/Hatrack.exe --smoke "$PWD/artifacts/Hatrack-smoke-new"
```

The website was removed in 0.2.0; its guides are in `docs/guides`.

## Required before a stable release

- Independent sign-in, sign-out, credentials, and restart persistence for two accounts in **both desktop apps** remain unverified. Separate directories and windows alone cannot prove authenticated isolation.
- Fresh-machine installation, version upgrade, vendor-update behaviour, pinned taskbar customization, Narrator, keyboard-only onboarding, high contrast, and the full display-scaling matrix need the recorded checks in [ACCEPTANCE.md](ACCEPTANCE.md).
- Transactional creation provides atomic metadata commits and rollback for caught failures. It is not a durable cross-file transaction after power loss; interrupted creation needs acceptance testing before stable publication.
- Detected vendor versions outside the embedded evidence ask for confirmation once per version before launch. Re-test and update the evidence through a new Hatrack release; do not erase data or downgrade vendor apps as a repair.
- Installer signatures and public download URLs require the owner's publication steps. No release or website has been publicly published by this implementation.

Compare the installer against `dist/SHA256SUMS.txt` before distribution. Retain signing status and these limitations in release notes.

## 0.1.1 installer

The 0.1.1 installer passed isolated install, reinstall, self-contained launch, and data-preserving uninstall. SHA-256:

```text
4fc9ebb11a82fee93e6434e57655c631ad9d177579150c1e5e9ad79ab5e79547
```

### Icon sizing correction | 4 October 2026

The colourable app marks now use the full icon canvas at default zoom, matching the existing Claude shortcuts instead of adding 24 pixels of padding on each side. The 16 engine checks passed again. The installer was rebuilt; installer lifecycle checks above apply to the preceding build and were not repeated for this artwork change.

Updated unsigned installer SHA-256:

```text
6c8af53a0c1fe3ebdd24ccf01a80e31890cc7e1b16f50b204a5f00e4196ebf9a
```

## 0.2.0 installer: rename to Hatrack | 4 October 2026

Built with .NET SDK 10.0.401 and Inno Setup 6 on Windows 11 x64. All 18 engine checks passed, including three new ones: untested vendor versions need one confirmation per version, a pre-rename `%LOCALAPPDATA%\Profiles` catalogue keeps being used until a Hatrack catalogue exists, and pre-rename shortcuts that target `Profiles.exe` stay owned and are repaired to `Hatrack.exe`. The isolated installer lifecycle (install, reinstall, self-contained launch, data-preserving uninstall) passed. The installer keeps the pre-rename AppId so an existing install upgrades in place and runs `--repair-shortcuts`.

Not yet verified: upgrading a real 0.1.1 install with existing profile shortcuts on a separate machine. Authenticated isolation remains unverified as above.

Unsigned installer SHA-256:

```text
4a925f144bd6bc2aaf0cd5af53e8cc509c5ed764c1e6d829a763c2e1ef6ff53f
```
