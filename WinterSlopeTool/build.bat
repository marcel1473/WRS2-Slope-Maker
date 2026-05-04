@echo off
setlocal

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:WinterSlopeTool.exe /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll Program.cs

if errorlevel 1 (
  echo Build failed.
  exit /b 1
)

echo Built WinterSlopeTool.exe
