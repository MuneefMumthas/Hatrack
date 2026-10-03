# Use a second Claude account on Windows without logging out

Claude Desktop keeps one account signed in at a time. Here is how to keep a personal and a work account open side by side, and switch with one click.

## Why Claude Desktop logs you out

Claude Desktop stores your login, conversations and settings in one data folder on your PC. Signing in to another account replaces the first one, because both would share that folder.

## The fix: one data folder per account

Give each account its own data folder and Claude Desktop treats them as separate installs. Hatrack does this for you: install it, choose Create profile, select Claude Desktop, name the profile (for example Work), and open it. Sign in to your second account in that window. Your first account is untouched.

## Switch accounts in one click

Each profile gets its own Desktop and Start Menu shortcut and its own taskbar icon in a colour you choose. Click the one you need. Both can be open at the same time, so there is nothing to sign out of.

## Other ways, and their limits

Signing out and back in works but costs time on every switch. Browser profiles keep several accounts open, but only for claude.ai on the web. Separate Windows users keep accounts apart, but you can only use one at a time. Hand-made shortcuts with the --user-data-dir flag work, but their windows merge on the taskbar and are easy to break.

## What this preview verifies

Separate data folders and separate taskbar identities are implemented and tested. Independent sign-in across Claude Desktop updates is still going through acceptance checks, and Hatrack asks once before opening a profile on an untested Claude version.

---

[Download Hatrack](https://github.com/MuneefMumthas/hatrack/releases/latest) · [All guides](README.md) · [Back to the project](../../README.md)
