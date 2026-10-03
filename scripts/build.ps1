param([string]$Version='0.2.0',[string]$Dotnet='dotnet',[string]$Iscc='C:\Program Files (x86)\Inno Setup 6\ISCC.exe',[switch]$Stable)
$ErrorActionPreference='Stop'
$env:DOTNET_CLI_TELEMETRY_OPTOUT='1'
$root=Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
    if($Stable){$evidence=Get-Content -LiteralPath 'compatibility.json' -Raw | ConvertFrom-Json;if(-not $evidence.releaseReady -or -not $evidence.apps.Claude.independentAuthenticationVerified -or -not $evidence.apps.Codex.independentAuthenticationVerified){throw 'Stable publication is blocked: desktop authentication and acceptance evidence is incomplete.'}}
    if(-not (Test-Path -LiteralPath $Iscc)){throw 'Install Inno Setup 6 or pass -Iscc with its ISCC.exe location.'}
    & $Dotnet run --project tests/Hatrack.Tests -c Release
    if($LASTEXITCODE -ne 0){throw 'Hatrack safety checks failed.'}
    & $Dotnet publish src/Hatrack/Hatrack.csproj -c Release -r win-x64 --self-contained true -o artifacts/publish -p:Version=$Version
    if($LASTEXITCODE -ne 0){throw 'Desktop publish failed.'}
    Copy-Item -LiteralPath LICENSE,THIRD-PARTY-NOTICES.md,compatibility.json -Destination artifacts/publish
    Copy-Item -LiteralPath assets/vendor/README.md -Destination artifacts/publish/APP-ICON-SOURCES.md
    $runtime=Get-Content -LiteralPath artifacts/publish/Hatrack.runtimeconfig.json -Raw | ConvertFrom-Json
    $packages=if($env:NUGET_PACKAGES){$env:NUGET_PACKAGES}else{Join-Path ([Environment]::GetFolderPath('UserProfile')) '.nuget\packages'}
    $notices=Join-Path $root 'artifacts\publish\notices'
    New-Item -ItemType Directory -Path $notices -Force | Out-Null
    foreach($framework in $runtime.runtimeOptions.includedFrameworks){
        $packageName=if($framework.name -eq 'Microsoft.NETCore.App'){'microsoft.netcore.app.runtime.win-x64'}else{'microsoft.windowsdesktop.app.runtime.win-x64'}
        $package=Join-Path (Join-Path $packages $packageName) $framework.version
        $files=Get-ChildItem -LiteralPath $package -File | Where-Object {$_.Name -match 'LICENSE|NOTICE'}
        if(-not $files){throw "Missing runtime licence notices for $packageName"}
        foreach($file in $files){Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $notices ($packageName+'-'+$file.Name))}
    }
    & $Iscc /Q "/DBuildVersion=$Version" installer/Hatrack.iss
    if($LASTEXITCODE -ne 0){throw 'Installer compilation failed.'}
    if($env:HATRACK_SIGNTOOL -and $env:HATRACK_CERTIFICATE_THUMBPRINT){
        & $env:HATRACK_SIGNTOOL sign /sha1 $env:HATRACK_CERTIFICATE_THUMBPRINT /fd SHA256 /tr 'http://timestamp.digicert.com' /td SHA256 'dist\Hatrack-Setup.exe'
        if($LASTEXITCODE -ne 0){throw 'Installer signing failed.'}
    }
    Copy-Item -LiteralPath THIRD-PARTY-NOTICES.md -Destination dist
    $hash=(Get-FileHash -LiteralPath 'dist\Hatrack-Setup.exe' -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  Hatrack-Setup.exe" | Set-Content -LiteralPath 'dist\SHA256SUMS.txt' -Encoding ascii
    Write-Output "Built dist/Hatrack-Setup.exe ($Version). Data and existing installations have not been changed."
} finally {Pop-Location}
