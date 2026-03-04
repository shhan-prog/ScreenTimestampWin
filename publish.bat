@echo off
chcp 65001 >nul 2>&1
echo ========================================
echo  ScreenTimestamp - Windows Build
echo ========================================
echo.

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET 8 SDK not installed.
    echo Download: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo [1/2] Building Release...
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish

if errorlevel 1 (
    echo [ERROR] Build failed.
    pause
    exit /b 1
)

echo.
echo [2/2] Build complete!
echo.
echo Output: %~dp0publish\ScreenTimestamp.exe
echo.
echo Copy this file to any Windows PC to run. No .NET install needed.
echo.
pause
