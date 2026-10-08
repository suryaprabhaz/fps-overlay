# SuryaHUD — FPS & System Overlay

A Windows desktop overlay focused on lightweight rendering, passive system monitoring and gaming-friendly presentation.

## Why it is interesting

Unlike process-injection overlays, SuryaHUD is designed around standard Windows APIs and a passive monitoring model. The project combines:

- C# / .NET 8
- WPF settings UI
- Win32 integration
- Direct3D / Direct2D rendering
- Real-time metric collection
- Automated unit tests
- Windows CI

## Engineering goals

The important metric is not a marketing claim such as “sub-millisecond” or “<2% CPU”; it is **measured overhead on real hardware**. Releases should record CPU usage, memory usage, update rate and frame-time impact.

See [docs/architecture.md](docs/architecture.md) for the system design and [CONTRIBUTING.md](CONTRIBUTING.md) for development standards.

## Build

Requires the .NET 8 SDK and Windows.

```bash
dotnet restore SuryaHUD.sln
dotnet build SuryaHUD.sln --configuration Release
dotnet test SuryaHUD.sln --configuration Release
```

For a distributable build, use `publish.bat`.

## Author

[@suryaprabhaz](https://github.com/suryaprabhaz)
