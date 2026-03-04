@echo off
chcp 65001 >nul 2>&1
echo ========================================
echo  ScreenTimestamp - Installer Build
echo ========================================
echo.

echo [1/3] Building Release...
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish

if errorlevel 1 (
    echo [ERROR] Build failed. Check .NET 8 SDK is installed.
    pause
    exit /b 1
)

echo [2/3] Build complete: publish\ScreenTimestamp.exe
echo.

echo [3/3] Creating installer...

set ISCC=
if exist "%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" (
    set "ISCC=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
)
if exist "%ProgramFiles%\Inno Setup 6\ISCC.exe" (
    set "ISCC=%ProgramFiles%\Inno Setup 6\ISCC.exe"
)

if "%ISCC%"=="" (
    echo.
    echo [INFO] Inno Setup 6 not found. Only .exe was created.
    echo.
    echo  Output: publish\ScreenTimestamp.exe
    echo.
    echo To create an installer, install Inno Setup 6:
    echo https://jrsoftware.org/isinfo.php
    echo.
    pause
    exit /b 0
)

"%ISCC%" installer.iss

if errorlevel 1 (
    echo [ERROR] Installer creation failed.
    pause
    exit /b 1
)

echo.
echo ========================================
echo  Done!
echo ========================================
echo.
echo  EXE:       publish\ScreenTimestamp.exe
echo  Installer: installer_output\ScreenTimestamp_Setup.exe
echo.
pause
