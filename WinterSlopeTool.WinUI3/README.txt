Winter Resort Simulator 2 - Slope Adder
WinUI 3 / Windows App SDK 2.0.1 version

Requirements to build:
- Visual Studio 2022 with .NET desktop development
- Windows App SDK C# templates/component
- .NET SDK 8 or newer

This project pins:
Microsoft.WindowsAppSDK 2.0.1

Build:
dotnet build .\WinterSlopeTool.WinUI3.csproj -c Release -r win-x64

Publish:
dotnet publish .\WinterSlopeTool.WinUI3.csproj -c Release -r win-x64

How to use:
1. Run the app.
2. Browse to your Winter Resort Simulator savegame .lua file.
3. Fill in slope id, Info Layer, run name, from/to, difficulty, length, and capacity.
4. Click Add Slope To File.

The app creates a backup next to the save file before editing it.
