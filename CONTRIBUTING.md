# Contributing

Hatrack manages Windows desktop sessions, not CLI profiles. Use Windows 11 x64 and .NET 10 SDK. Run the isolated checks before submitting a change:

```powershell
dotnet run --project tests/Hatrack.Tests -c Release
```

For the website, use Node 22.12+, run `npm ci` in `website`, then `npm run build`. Build the installer with `scripts/build.ps1` and Inno Setup 6.

Tests use a disposable temporary catalogue and shortcuts; never test destructive operations against your real account folders. Do not commit tokens, local diagnostics, user history, uploaded private artwork, or profile catalogues.

Include the problem, change, and verification evidence in a pull request. Profile IDs must remain stable, unknown shortcuts must never be overwritten, and removal must preserve data. App-version support requires actual independent desktop-session evidence, not a successful CLI launch.

Contributions are provided under the repository's MIT licence. Preserve attribution for third-party code and use original, appropriately licensed artwork.
