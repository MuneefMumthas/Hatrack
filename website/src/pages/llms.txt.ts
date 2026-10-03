export const GET=({site}:any)=>{const base=import.meta.env.BASE_URL.replace(/\/$/,'');const u=(p:string)=>new URL(base+p,site).toString();const repo=process.env.REPOSITORY_URL||'https://github.com/MuneefMumthas/hatrack';return new Response(`# Hatrack

> Free, open-source Windows 11 app for running multiple Claude Desktop and Codex Desktop accounts. Each account gets its own profile (separate data folder), Desktop and Start Menu shortcut, and taskbar icon, so a second account can be used without logging out of the first. Preview release. Desktop apps only, not Claude Code or Codex CLI. Not affiliated with Anthropic or OpenAI.

- [Download](${repo}/releases/latest): Hatrack-Setup.exe for Windows 11 x64
- [Source code](${repo}): MIT licence
- [Use a second Claude account without logging out](${u('/guides/second-claude-account/')})
- [Multiple Claude Desktop accounts](${u('/guides/claude-desktop/')})
- [Multiple Codex Desktop accounts](${u('/guides/codex-desktop/')})
- [Troubleshooting](${u('/guides/troubleshooting/')})
- [Privacy](${u('/guides/privacy/')})
`,{headers:{'Content-Type':'text/plain; charset=utf-8'}});};
