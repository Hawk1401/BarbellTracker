; Inno Setup script for the BarbellTracker installer.
; Build (see .github/workflows/dotnet.yml):
;   dotnet publish src\BarbellTracker.WPF_DesktopClient -c Release -r win-x64 --self-contained -p:Version=1.0.0 -o artifacts\publish
;   iscc /DMyAppVersion=1.0.0 /DPublishDir=..\artifacts\publish installer\BarbellTracker.iss
; The installer is written to artifacts\.

#ifndef MyAppVersion
  #define MyAppVersion "1.0.0"
#endif
#ifndef PublishDir
  #define PublishDir "..\artifacts\publish"
#endif

#define MyAppName "BarbellTracker"
#define MyAppExe "BarbellTracker.WPF_DesktopClient.exe"

[Setup]
AppId={{EF74EAEE-5EBE-48C7-8E68-5AADD3D2D40E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=Florian Koch, Jan Metzger
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExe}
OutputDir=..\artifacts
OutputBaseFilename=BarbellTracker-Setup-{#MyAppVersion}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
; per user installation by default, no administrator rights needed
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog

[Languages]
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExe}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent

; The sessions, settings and logs in %AppData%\BarbellTracker are intentionally NOT removed on uninstall.
