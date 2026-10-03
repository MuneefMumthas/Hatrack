# Launch plan: getting Hatrack in front of people

Stars follow attention, and attention follows a clear demo shown to people who already have the problem. Plenty of people have it: Anthropic's own tracker has an issue titled [“desktop app desperately needs multi account support”](https://github.com/anthropics/claude-code/issues/36821). Aim for one concentrated launch day, then keep a steady release rhythm.

## Before launch day

- [ ] Published prerelease with `Hatrack-Setup.exe` ([PUBLISHING.md](PUBLISHING.md)).
- [ ] Social preview uploaded and topics set.
- [ ] **A 20–30 second demo.** This is the single biggest lever. Record with [ScreenToGif](https://www.screentogif.com/) or the Windows Snipping Tool: install, create a "Work" profile, recolour its icon, open it next to "Personal", and show both taskbar icons. Use disposable accounts and hide email addresses. Export an MP4 for LinkedIn and X, and a GIF under 10 MB for the top of the README.
- [ ] Pin the repository on your GitHub profile and add one line about it to your profile README.
- [ ] Label two or three small issues `good first issue` so visitors have a way in.
- [ ] Ask two friends to install it on clean machines and report anything confusing.

## Launch day (Tuesday to Thursday, start around 14:00 UK / 09:00 US Eastern)

GitHub Trending ranks by stars gained in a short window, so post everywhere on the same day and stay online to answer comments.

1. **Hacker News**: Show HN (template below). Reply to every comment, especially critical ones.
2. **LinkedIn**: the post below, with the video uploaded natively.
3. **Reddit**: one subreddit at a time, following each one's self-promotion rules. Good fits: r/ClaudeAI, r/OpenAI, r/ChatGPTCoding, r/Windows11, r/opensource, r/coolgithubprojects, r/SideProject.
4. **X, Bluesky, Mastodon**: the short post with the video.
5. **Where the demand already is**: post one helpful reply on the [multi-account issue](https://github.com/anthropics/claude-code/issues/36821) and similar threads, saying that you built it. Do it once per thread; don't repeat it.

## The week after

- Write a technical post on dev.to, Hashnode or your own site: *How to run multiple Claude Desktop accounts on Windows*. Cover `--user-data-dir`, `CODEX_HOME`, AppUserModelIDs and why taskbar icons merge. Link the repository. Technical write-ups rank in search and get shared.
- Submit pull requests to relevant curated lists, only where Hatrack fits their rules: awesome-windows, awesome-dotnet, awesome-wpf, and Claude/OpenAI tool lists.
- List it on [AlternativeTo](https://alternativeto.net) as an alternative to manual multi-account workarounds.
- **winget**: once a release URL is stable, run `wingetcreate new <installer URL>` and open the pull request it prepares against `microsoft/winget-pkgs`. Then people can run `winget install Hatrack`.

## Ongoing

- Release small improvements every two to four weeks. Each release appears in watchers' feeds and gives you something new to post.
- Reply to issues within a day. Thank people who report bugs, and credit contributors in release notes.
- Test every new Claude and Codex Desktop version and ship a compatibility update quickly. Being the tool that works today is the moat.
- Product Hunt: save it for the first stable, signed release.

## Don't

Don't buy stars, join star-for-star exchanges, mass-DM people, or post the same text in many places at once. GitHub removes fake stars and can flag the account, and communities ban repeat self-promoters.

## Expectations

Fewer than a few hundred repositories on GitHub have 100,000 stars, and almost all of them are major frameworks or long-lived resources. A sharp Windows utility can realistically reach hundreds to a few thousand stars from a good launch, more if a post goes viral. Track milestones: 100, then 1,000, then a day on Trending. Each one makes the next easier.

---

## Ready-to-paste posts

Replace the bracketed links. Keep claims to what the preview has verified.

### LinkedIn

> I kept signing out of Claude Desktop to switch between my personal and work accounts. So I built a fix.
>
> **Hatrack** is a free, open-source Windows app that gives every Claude Desktop and Codex Desktop account its own profile: a separate data folder, its own shortcut, and its own taskbar icon in the colour you choose. Click the hat you need.
>
> What I cared about while building it:
> • Your login stays inside the official app. Hatrack never copies credentials.
> • No account, no telemetry, nothing uploaded.
> • It never overwrites a shortcut it didn't create, and removing a profile keeps its data.
>
> Built with .NET 10 and WPF, MIT licensed. It's a preview for Windows 11, and I'd love feedback from anyone juggling several AI accounts.
>
> Download and source: [github.com/MuneefMumthas/hatrack]
> If it saves you a few sign-ins, a GitHub star helps other people find it.
>
> #OpenSource #Windows #Claude #DeveloperTools #BuildInPublic

Tip: LinkedIn tends to show posts with outside links to fewer people. Upload the demo video natively and put the GitHub link in the first comment if you prefer.

### Show HN

**Title:** Show HN: Hatrack – Run multiple Claude Desktop and Codex accounts on Windows

> Claude Desktop and Codex Desktop sign in to one account at a time. I have personal and work accounts, so I was signing out and back in several times a day.
>
> Hatrack is a small open-source Windows app (.NET 10/WPF, MIT) that launches the official desktop apps with a separate data directory per profile: `--user-data-dir` for Claude, and `CODEX_HOME` plus Electron user data for Codex. Each profile gets its own shortcut, icon and AppUserModelID, so the taskbar keeps them apart.
>
> It's a preview: separate local state works, and I'm still verifying independent sign-in across vendor updates. It has no telemetry or account, and it never copies credentials. Feedback on the approach, and on edge cases I've missed, is very welcome.
>
> [repository link]

### Reddit

**Title:** I made a free Windows app to run multiple Claude Desktop (and Codex) accounts side by side

> Switching between personal and work accounts in Claude Desktop meant signing out every time. Hatrack gives each account its own profile: a separate data folder, shortcut and taskbar icon. Open source (MIT), no telemetry, sign-in stays in the official app. Windows 11 preview; I'd like to hear what breaks. [link]

### X / Bluesky

> Built Hatrack: a free, open-source Windows app to run multiple Claude Desktop and Codex accounts, each with its own profile, shortcut and taskbar icon. No telemetry, never copies your login. [link] [video]
