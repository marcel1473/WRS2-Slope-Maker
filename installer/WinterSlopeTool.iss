#define MyAppName "Winter Slope Tool"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Marcel"
#define MyAppExeName "WinterSlopeTool.exe"

[Setup]
AppId={{BCA8D635-1A20-4C6A-A75E-A72D62B7B409}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\Winter Slope Tool
DefaultGroupName=Winter Slope Tool
DisableProgramGroupPage=yes
OutputDir=..\release
OutputBaseFilename=WinterSlopeTool-Setup-v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=lowest
SetupIconFile=..\WinterSlopeTool.WinUI3\Assets\AppIcon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Files]
Source: "..\release\WinterSlopeTool-v1.0.0-win-x64-portable\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Winter Slope Tool"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\Winter Slope Tool"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch Winter Slope Tool"; Flags: nowait postinstall skipifsilent
