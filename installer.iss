; Inno Setup Script - ScreenTimestamp Windows Installer
; Inno Setup 다운로드: https://jrsoftware.org/isinfo.php

[Setup]
AppName=ScreenTimestamp
AppVersion=1.0.0
AppPublisher=ScreenTimestamp
DefaultDirName={autopf}\ScreenTimestamp
DefaultGroupName=ScreenTimestamp
UninstallDisplayIcon={app}\ScreenTimestamp.exe
OutputDir=installer_output
OutputBaseFilename=ScreenTimestamp_Setup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "publish\ScreenTimestamp.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\ScreenTimestamp"; Filename: "{app}\ScreenTimestamp.exe"
Name: "{group}\ScreenTimestamp 제거"; Filename: "{uninstallexe}"
Name: "{userstartup}\ScreenTimestamp"; Filename: "{app}\ScreenTimestamp.exe"; Comment: "시작 시 자동 실행"

[Run]
Filename: "{app}\ScreenTimestamp.exe"; Description: "ScreenTimestamp 실행"; Flags: nowait postinstall skipifsilent
