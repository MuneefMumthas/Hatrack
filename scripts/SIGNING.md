# Optional Windows signing

Install the Windows SDK SignTool and configure a code-signing certificate you own. The build script supports `HATRACK_SIGNTOOL` (absolute SignTool executable path) and `HATRACK_CERTIFICATE_THUMBPRINT` (certificate thumbprint available to the build user). It signs the installer using SHA-256 and a timestamp server.

For stable distribution also sign `Hatrack.exe` before installer compilation; adapt the build pipeline to your certificate provider or hardware-backed signing service. Do not commit certificates or passwords. Verify signature status on release assets and describe it in release notes. A signed build is not proof of desktop isolation or absence of defects.

Initial local builds are unsigned. Signing must not bypass compatibility acceptance or automatically publish a release.
