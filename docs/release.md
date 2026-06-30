# Release

Emergency Stop uses Velopack for Windows installation and automatic updates.

## Release Flow

1. Push a version tag such as `v0.1.0`, or run the `Release` workflow manually.
2. GitHub Actions publishes the Windows x64 app.
3. Velopack builds the installer, portable package, update packages, and release feed.
4. The workflow uploads the generated files to GitHub Releases.
5. Prerelease versions such as `v0.2.0-beta.1` are uploaded as GitHub prereleases.

## Auto Update Behavior

- Automatic update checks run on startup when `Automatic updates` is enabled.
- Updates are available only for builds installed through the Velopack `Setup.exe`.
- Portable or developer builds skip update checks safely.
- The app downloads available updates in the background, then asks before restarting to install.
- The tray menu also has `Check for updates` for manual checks.

## Local Packaging

```powershell
dotnet tool install --global vpk --version 1.2.0
dotnet publish src\EmergencyStop\EmergencyStop.csproj -c Release -r win-x64 --self-contained true -o artifacts\publish -p:Version=0.1.0 -p:PublishSingleFile=false -p:DebugType=none -p:DebugSymbols=false
vpk --yes true --legacyConsole true pack --packId EmergencyStop --packVersion 0.1.0 --packDir artifacts\publish --outputDir artifacts\velopack --mainExe EmergencyStop.exe --packTitle "Emergency Stop" --packAuthors "Ninthless" --runtime win-x64 --channel win --shortcuts StartMenuRoot
```

## Notes

- Do not upload portable single-file builds as the primary release path once Velopack is active.
- Keep release tags semantic, for example `v0.1.0` or `v0.2.0-beta.1`.
- Stable installations check stable GitHub Releases by default.
- Add code signing later with Velopack `--signParams`, `--signTemplate`, or Azure Trusted Signing.
