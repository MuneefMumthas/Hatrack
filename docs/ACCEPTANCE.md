# Stable-release acceptance

Only mark `releaseReady: true` after recording all applicable results with the exact installer, Windows version, and vendor package versions. A preview installer and passing unit checks do not certify account separation.

- [ ] Fresh Windows 11 x64 installation without .NET/Node/Git succeeds.
- [ ] Reinstall, upgrade, uninstall, and reinstall preserve profile IDs and data.
- [ ] Existing Claude Work, Business, Yellow, and original shortcuts remain byte-identical during detection and unrelated creation.
- [ ] Two Claude desktop profiles open simultaneously; each signs into a different disposable account.
- [ ] Signing out of one Claude profile leaves the other and original session signed in.
- [ ] Repeat simultaneous sign-in/sign-out and restart-persistence checks for two Codex desktop profiles.
- [ ] Codex agent state, browser state, sessions, and file credentials use profile-local directories.
- [ ] Vendor updates preserve independent state and resolve the current installation.
- [ ] Desktop, Start, window, and pinned taskbar identity match per profile after rename and icon edit.
- [ ] Unknown shortcut collision rolls back without modifying the existing file.
- [ ] Manual desktop executable detection rejects CLI tools and ordinary ChatGPT Desktop.
- [ ] Missing-app installation resumes detection correctly.
- [ ] Imported profiles retain original data and shortcuts; unsupported credential stores fail closed.
- [ ] Keyboard-only flow, Narrator labels, high contrast, 100/150/200% scaling, light/dark themes verified.
- [ ] Uploaded malformed and oversized images fail safely; supported images preserve transparency and crop.
- [ ] README screenshots are current; download links point to real release assets; claims match actual support.
- [ ] Signing status, known limitations, checksums, and notices documented.

Record independent-authentication verification for each app in `compatibility.json`. A release manager must review the evidence; changing a boolean without evidence does not satisfy this gate.
