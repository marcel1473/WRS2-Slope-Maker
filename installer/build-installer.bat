@echo off
setlocal

where iscc >nul 2>nul
if errorlevel 1 (
  echo Inno Setup Compiler was not found.
  echo Install Inno Setup, then run this file again.
  exit /b 1
)

iscc WinterSlopeTool.iss
