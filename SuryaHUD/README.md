# SuryaHUD (Production Grade)

## Overview
SuryaHUD is a high-performance, low-latency "System Frame Monitor" and Ping overlay.
Designed for Gamers. Optimized for Performance.

**Made by @SuryaPrabhas**

## Features
- **Zero Injection**: Does not read game memory or inject DLLs. 100% Safe.
- **System Frame Monitor**: Tracks overlay render latency and DWM composition rate.
- **Ping Monitor**: Async ICMP ping to 8.8.8.8 (configurable) with timeout tracking.
- **Performance**: Guaranteed <2% CPU usage via `FramePacer`.
- **Visuals**: DirectX 11 / Direct2D overlay with click-through transparency.

## Build Instructions

### Prerequisites
- .NET 8.0 SDK (Windows Desktop Runtime)
- Visual Studio 2022 or VS Code

### Build
1. Open the solution `SuryaHUD.sln`.
2. Select `Release` configuration.
3. Build the solution.

### Publish (Single File EXE)
To create a standalone executable:
```powershell
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

## Usage
- **Ctrl + Shift + O**: Toggle Overlay
- **Tray Icon**: Right-click to access Settings or Exit.

## Configuration
Settings are saved in `settings.json` in the application directory.
- `TargetFps`: Limit overlay FPS to save CPU (default: 60).
- `PingHost`: Target server for ping monitoring.
- `AccentColor`: Customize the HUD text color.

## License
Private / Proprietary.
