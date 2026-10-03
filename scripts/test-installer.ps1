param([string]$Installer=(Join-Path (Split-Path $PSScriptRoot -Parent) 'dist\Hatrack-Setup.exe'))
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$testRoot=Join-Path $root ('artifacts\Hatrack-install-test-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $testRoot | Out-Null
$accountRoot=Join-Path $testRoot 'account-data'
New-Item -ItemType Directory -Path $accountRoot | Out-Null
$sentinel=Join-Path $accountRoot 'sentinel.txt'
'preserve' | Set-Content -LiteralPath $sentinel
$installRoot=Join-Path $testRoot 'application'
$args=@('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART','/TESTMODE=1',('/DIR="'+$installRoot+'"'))
try {
    foreach($phase in @('install','reinstall')) {
        $p=Start-Process -FilePath $Installer -ArgumentList $args -WindowStyle Hidden -Wait -PassThru
        if($p.ExitCode -ne 0 -or -not(Test-Path -LiteralPath (Join-Path $installRoot 'Hatrack.exe'))){throw "$phase failed"}
        Write-Output "PASS $phase"
    }
    $env:HATRACK_TEST_HOME=$accountRoot
    $p=Start-Process -FilePath (Join-Path $installRoot 'Hatrack.exe') -ArgumentList @('--capture-light',('"'+(Join-Path $testRoot 'installed-ui.png')+'"')) -WindowStyle Hidden -Wait -PassThru
    if($p.ExitCode -ne 0){throw 'Installed self-contained executable failed'}
    $catalog=Join-Path $accountRoot 'catalog.json'
    $hash=(Get-FileHash -LiteralPath $catalog).Hash
    $u=Start-Process -FilePath (Join-Path $installRoot 'unins000.exe') -ArgumentList @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART','/TESTMODE=1') -WindowStyle Hidden -Wait -PassThru
    if($u.ExitCode -ne 0){throw 'Uninstall failed'}
    if((Get-FileHash -LiteralPath $catalog).Hash -ne $hash -or (Get-Content -LiteralPath $sentinel) -ne 'preserve'){throw 'Profile data changed during uninstall'}
    Write-Output 'PASS installed executable and data-preserving uninstall'
} finally {Remove-Item Env:HATRACK_TEST_HOME -ErrorAction SilentlyContinue}
Write-Output "Evidence retained at $testRoot"
