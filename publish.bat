@echo off
echo checking for .NET SDK...
dotnet --list-sdks
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] .NET SDK not found!
    echo You must install the .NET 8.0 SDK to build this application.
    echo Download it here: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
    echo.
    pause
    exit /b
)

echo.
echo Building SuryaHUD as Single-File EXE...
echo.

pushd "%~dp0"

echo Closing any running instances of SuryaHUD...
taskkill /F /IM SuryaHUD.exe >nul 2>&1

cd SuryaHUD
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ../Build/Release

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Build Failed!
    pause
    exit /b
)

echo.
echo Build Successful!
echo Executable is located in: Build\Release\SuryaHUD.exe
echo.
pause
