#ifndef BuildVersion
  #define BuildVersion "0.2.0"
#endif
[Setup]
; AppId is unchanged from the pre-rename Profiles installer so existing installs upgrade in place.
AppId={code:InstallerAppId}
UsePreviousLanguage=no
UsePreviousGroup=no
AppName=Hatrack
AppVersion={#BuildVersion}
AppPublisher=Muneef Mumthas
DefaultDirName={localappdata}\Programs\Hatrack
DefaultGroupName=Hatrack
PrivilegesRequired=lowest
MinVersion=10.0.22000
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=..\dist
OutputBaseFilename=Hatrack-Setup
SetupIconFile=..\assets\hatrack.ico
UninstallDisplayIcon={app}\Hatrack.exe
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
DisableProgramGroupPage=yes
DisableDirPage=yes
LicenseFile=..\LICENSE
CloseApplications=yes
RestartApplications=no
[InstallDelete]
; Pre-rename executable and the installer's own old manager shortcuts. Profile shortcuts are repaired, not deleted.
Type: files; Name: "{app}\Profiles.*"
Type: files; Name: "{autodesktop}\Profiles.lnk"
Type: files; Name: "{userprograms}\Profiles\Profiles.lnk"
[Files]
Source: "..\artifacts\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
[Icons]
Name: "{group}\Hatrack"; Filename: "{app}\Hatrack.exe"; AppUserModelID: "Hatrack.Manager"; Check: not IsTestMode
Name: "{autodesktop}\Hatrack"; Filename: "{app}\Hatrack.exe"; AppUserModelID: "Hatrack.Manager"; Check: not IsTestMode
[Run]
Filename: "{app}\Hatrack.exe"; Parameters: "--repair-shortcuts"; Flags: runhidden waituntilterminated; Check: not IsTestMode
Filename: "{app}\Hatrack.exe"; Description: "Open Hatrack"; Flags: nowait postinstall skipifsilent; Check: not IsTestMode
[UninstallRun]
Filename: "{app}\Hatrack.exe"; Parameters: "--uninstall-shortcuts"; Flags: runhidden waituntilterminated; RunOnceId: "RemoveOwnedShortcuts"; Check: not IsTestMode
; Account data and catalogue live outside {app} and are intentionally preserved.
[Code]
function IsTestMode: Boolean;
begin
  Result := ExpandConstant('{param:TESTMODE|0}') = '1';
end;
function InstallerAppId(Param: String): String;
begin
  Result := '{696724B2-7EDC-49A7-A478-AB37806A624B}';
  if IsTestMode then Result := Result + '-Test';
end;
