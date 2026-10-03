# Security reporting

Report credential disclosure, profile-state crossover, unintended file changes, or unsafe path handling privately using GitHub's **Report a vulnerability** feature when enabled. If it is unavailable, open an issue requesting a private contact without publishing sensitive details.

Do not attach authentication files, cookies, account identifiers, or conversations. Provide the Hatrack version, vendor desktop version, Windows version, and a minimal reproduction using disposable data.

Hatrack is a local-state organiser, not an operating-system security boundary. All profiles run as the same Windows user. A malicious application running as that user may access their files. Use separate Windows users for stronger separation.

Preview builds have not completed the stable compatibility gate. Keep verified support status in `compatibility.json`; do not infer support from a changed executable path.
