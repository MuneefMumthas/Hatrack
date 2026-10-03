# From zero to a published release, step by step

Everything below assumes the repository will be `github.com/MuneefMumthas/hatrack` and that you run commands in **PowerShell** from the project folder (`Hatrack`). Each step says what you should see. Stop and fix anything that doesn't match before moving on.

---

## Part A: check everything on your PC (10 minutes)

### A1. Open PowerShell in the project folder

In File Explorer, open the `Hatrack` folder, click the address bar, type `powershell` and press Enter.

### A2. Run the tests

```powershell
$env:DOTNET_ROOT="$PWD\.tools\dotnet"
.\.tools\dotnet\dotnet.exe run --project tests/Hatrack.Tests -c Release
```

You should see `18 checks passed`. (The `.tools` folder holds the .NET 10 SDK because your system only has .NET 9. Alternatively, install it with `winget install Microsoft.DotNet.SDK.10` and use plain `dotnet`.)

### A3. Build the installer

```powershell
powershell -ExecutionPolicy Bypass -File scripts\build.ps1 -Dotnet "$PWD\.tools\dotnet\dotnet.exe"
powershell -ExecutionPolicy Bypass -File scripts\test-installer.ps1
```

You should see `Built dist/Hatrack-Setup.exe (0.2.0)` and then three `PASS` lines.

### A4. Try it yourself

Run `dist\Hatrack-Setup.exe`. It upgrades your existing Profiles install. Open Hatrack, check that your existing profiles are listed, and open one from its Desktop shortcut.

---

## Part B: put the code on GitHub (5 minutes)

### B1. Confirm you're signed in to GitHub CLI

```powershell
gh auth status
```

You should see `Logged in to github.com account MuneefMumthas`. If not, run `gh auth login`, choose GitHub.com, HTTPS, and log in with a web browser.

### B2. Make the first commit

```powershell
git add -A
git status
```

Read the list. It must **not** include `catalog.json`, `auth.json`, `.tools/`, `dist/`, `artifacts/`, `node_modules/` or `.claude/`. (`.gitignore` already excludes them.) Then:

```powershell
git commit -m "Hatrack 0.2.0 preview"
```

### B3. Create the public repository and upload

```powershell
gh repo create MuneefMumthas/hatrack --public --source . --remote origin --push --description "Run multiple Claude Desktop and Codex accounts on Windows. Open a second account without signing out: separate profiles, shortcuts and taskbar icons. Free and open source."
```

Open https://github.com/MuneefMumthas/hatrack. You should see the README with the hat logo.

*Without GitHub CLI:* on github.com click **+ → New repository**, name it `hatrack`, choose **Public**, leave every "Add a README/licence/.gitignore" box **unticked**, click **Create repository**, then run:

```powershell
git remote add origin https://github.com/MuneefMumthas/hatrack.git
git push -u origin main
```

---

## Part C: settings that help people find it (10 minutes)

The repository description and README are what Google shows for GitHub results, so they now lead with the phrases people search for.

### C1. Topics and Discussions

```powershell
gh repo edit MuneefMumthas/hatrack --enable-discussions --add-topic claude,claude-desktop,claude-ai,codex,codex-desktop,openai-codex,multiple-accounts,multi-account,account-switcher,profile-manager,windows,windows-11,desktop-app,wpf,dotnet,open-source
```

### C2. Social preview image

On the repository page: **Settings → General → Social preview → Edit → Upload an image**, and choose `docs\images\social-preview.png`. This is the card shown when the link is shared on LinkedIn, X, Slack and similar.

### C3. Security reporting

**Settings → Code security → Private vulnerability reporting → Enable.**

---

## Part D: publish the release (5 minutes)

Releases are built on your PC and uploaded with GitHub CLI. The repository has no GitHub Actions workflows, so pushes and tags never trigger runs.

### D1. Build and test (Part A)

You need `dist\Hatrack-Setup.exe` and `dist\SHA256SUMS.txt` from a run of A2 to A3 that passed.

### D2. Tag the version

```powershell
git tag -a v0.2.0-preview.1 -m "Hatrack 0.2.0 preview 1"
git push origin v0.2.0-preview.1
```

### D3. Upload and publish

Write the release notes in a file such as `release-notes.md` (example below; don't commit it), then:

```powershell
gh release create v0.2.0-preview.1 dist\Hatrack-Setup.exe dist\SHA256SUMS.txt dist\THIRD-PARTY-NOTICES.md compatibility.json docs\BUILD-VERIFICATION.md --verify-tag --latest --title "Hatrack v0.2.0 preview" --notes-file release-notes.md
```

`--latest` without `--prerelease` keeps the README's **Download** button working, because GitHub's "latest" link skips pre-releases. The notes still say it's a preview.

```markdown
## Hatrack 0.2.0 preview

Run multiple Claude Desktop and Codex Desktop accounts on Windows 11. Use a second account without signing out of the first.

**Download:** Hatrack-Setup.exe below. No .NET, Node or terminal needed.

### Known limitations
- Preview: independent signed-in sessions are still going through acceptance checks.
- The installer is not code-signed. Windows SmartScreen may warn: More info, then Run anyway. Check the file against SHA256SUMS.txt.
```

### D4. Check it

Open https://github.com/MuneefMumthas/hatrack/releases/latest and click `Hatrack-Setup.exe`. It should download.

---

## Part E: getting found on Google (no website needed)

Google indexes GitHub repositories, READMEs and the Markdown files in `docs/guides`. What helps:

1. **The description and topics** (B3, C1). Google uses the description as the snippet for the repo.
2. **Links to the repo.** Every launch post, article and forum answer that links to `github.com/MuneefMumthas/hatrack` helps it rank. See [LAUNCH.md](LAUNCH.md).
3. **Stars and activity.** Releases, issues and commits show the project is alive.
4. **Share card.** Paste the repository URL into LinkedIn's Post Inspector (https://www.linkedin.com/post-inspector/) to check the social preview image.

You can't submit a GitHub repo to Google Search Console, because you don't own `github.com`. Expect the repo to appear for searches like "Claude desktop second account Windows" a week or two after it starts getting links.

---

## Part F: every later release

1. Make your changes and update `CHANGELOG.md`.
2. Raise `<Version>` in `src/Hatrack/Hatrack.csproj` (for example `0.2.1`).
3. When you've tested a new Claude or Codex version, update `observedPackageVersion` in `compatibility.json`.
4. Run Part A, then commit and push:

```powershell
git add -A
git commit -m "Hatrack 0.2.1 preview"
git push
```

5. Repeat D2 to D4 with the new version number.

Never replace the installer inside a release that's already published. Publish a new version instead.

Next: [LAUNCH.md](LAUNCH.md) for getting the word out.
