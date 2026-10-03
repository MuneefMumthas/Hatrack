# From zero to a published release, step by step

Everything below assumes the repository will be `github.com/MuneefMumthas/hatrack` and that you run commands in **PowerShell** from the project folder (`profile creator`). Each step says what you should see. Stop and fix anything that doesn't match before moving on.

---

## Part A: check everything on your PC (10 minutes)

### A1. Open PowerShell in the project folder

In File Explorer, open the `profile creator` folder, click the address bar, type `powershell` and press Enter.

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
gh repo create MuneefMumthas/hatrack --public --source . --remote origin --push --description "Run multiple Claude Desktop and Codex accounts on Windows. Open a second account without signing out: separate profiles, shortcuts and taskbar icons. Free and open source." --homepage "https://muneefmumthas.github.io/hatrack/"
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

### C3. Turn on the website

1. **Settings → Pages → Build and deployment → Source:** choose **GitHub Actions**.
2. Open the **Actions** tab. Wait for **Website** and **Checks** to turn green (about 3 minutes). If **Website** ran before you changed the setting and failed, open it and click **Re-run all jobs**.
3. Visit https://muneefmumthas.github.io/hatrack/. You should see the Hatrack homepage.

### C4. Security reporting

**Settings → Code security → Private vulnerability reporting → Enable.**

---

## Part D: publish the release (10 minutes)

### D1. Tag the version

```powershell
git tag v0.2.0-preview.1
git push origin v0.2.0-preview.1
```

The `-preview` part matters. It makes GitHub build a pre-release. A plain `v1.0.0` tag refuses to build until signed-in isolation is verified, and that's on purpose.

### D2. Wait for the build

**Actions → Draft Windows release.** This takes about 10 minutes. It runs the tests, builds the installer, and installs and uninstalls it on a clean Windows machine. It must finish green.

### D3. Check the draft

Open **Releases** (right-hand side of the repository page). Click the draft **Hatrack v0.2.0-preview.1**. It should list `Hatrack-Setup.exe`, `SHA256SUMS.txt`, `THIRD-PARTY-NOTICES.md`, `compatibility.json` and `BUILD-VERIFICATION.md`. Download `Hatrack-Setup.exe` from there and install it once.

### D4. Write the notes and publish

Click **Edit** (pencil), replace the notes with the text below, keep **Set as a pre-release** ticked, and click **Publish release**.

```markdown
## Hatrack 0.2.0 preview

Run multiple Claude Desktop and Codex Desktop accounts on Windows 11. Use a second account without signing out of the first.

**Download:** Hatrack-Setup.exe below. No .NET, Node or terminal needed.

### What's new
- New name and logo (formerly Profiles). Existing profiles, shortcuts and taskbar pins keep working.
- Untested Claude or Codex versions now ask once instead of blocking.
- Lower background CPU use.

### Known limitations
- Preview: independent signed-in sessions are still going through acceptance checks.
- Tested with Claude Desktop 2.19675.0.0 and Codex Desktop 26.930.2377.0.
- The installer is not code-signed. Windows SmartScreen may warn: More info, then Run anyway. Check the file against SHA256SUMS.txt.
```

The README's **Download** link (`/releases/latest`) points to the newest full release, not to pre-releases. Until you publish a full release, people get there through **Releases**. If you want `/releases/latest` to work now, untick **Set as a pre-release**. The preview warning stays in the notes and the README.

---

## Part E: get indexed by Google and Bing (15 minutes, once)

GitHub pages get indexed on their own, usually within a few days of the repository getting its first links and stars. The website needs registering.

### E1. Google Search Console

1. Go to https://search.google.com/search-console and click **Add property**. Choose **URL prefix** and enter `https://muneefmumthas.github.io/hatrack/`.
2. Pick **HTML tag** verification. Copy only the `content="..."` value.
3. On GitHub: **Settings → Secrets and variables → Actions → Variables → New repository variable**. Name: `GOOGLE_SITE_VERIFICATION`. Value: the copied code.
4. **Actions → Website → Run workflow.** When it's green, click **Verify** in Search Console.
5. In Search Console, **Sitemaps** → submit `https://muneefmumthas.github.io/hatrack/sitemap.xml`.
6. **URL inspection** → paste the homepage URL → **Request indexing**. Repeat for `/hatrack/guides/second-claude-account/`.

### E2. Bing Webmaster Tools (also feeds DuckDuckGo, Yahoo and Copilot)

Go to https://www.bing.com/webmasters, sign in, and choose **Import from Google Search Console**. If you'd rather verify separately, use the meta tag method with a repository variable named `BING_SITE_VERIFICATION`, the same way as E1.

### E3. Optional: a root robots.txt

Crawlers only read `robots.txt` at the root of a domain. Yours (`muneefmumthas.github.io`) comes from the `MuneefMumthas.github.io` repository, which has none, so everything is allowed. That's fine. To point crawlers at Hatrack's sitemap as well, add a file named `robots.txt` to that repository containing:

```text
User-agent: *
Allow: /
Sitemap: https://muneefmumthas.github.io/hatrack/sitemap.xml
```

### E4. Check the share card

Paste the website URL into https://www.opengraph.xyz and the repository URL into LinkedIn's Post Inspector (https://www.linkedin.com/post-inspector/). Both should show the Hatrack card.

---

## Part F: every later release

1. Make your changes and update `CHANGELOG.md`.
2. Raise `<Version>` in `src/Hatrack/Hatrack.csproj` (for example `0.2.1`).
3. When you've tested a new Claude or Codex version, update `observedPackageVersion` in `compatibility.json`.
4. Run Part A, then:

```powershell
git add -A
git commit -m "Hatrack 0.2.1 preview"
git push
git tag v0.2.1-preview.1
git push origin v0.2.1-preview.1
```

5. Repeat D2 to D4.

Never replace the installer inside a release that's already published. Publish a new version instead.

Next: [LAUNCH.md](LAUNCH.md) for getting the word out.
