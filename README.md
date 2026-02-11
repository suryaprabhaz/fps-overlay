# 🎮 FPS Overlay (SuryaHUD)

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows%2010%2F11-lightgrey.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Status](https://img.shields.io/badge/status-stable-green.svg)

**FPS Overlay** (formerly SuryaHUD) is a production-grade, high-performance system monitor designed specifically for gamers and power users. Unlike traditional overlays (like MSI Afterburner or Discord) that inject code into game processes—risking anti-cheat bans—this tool uses a **safe, passive monitoring approach**.

It renders a lightweight, transparent layer over your screen to display:
*   **System Frame Rate**: Measurements from the Desktop Window Manager (DWM).
*   **Network Latency**: High-precision ICMP ping monitoring.

> **🛡️ 100% Anti-Cheat Safe**: No DLL injection. No memory hooking. No risk.

---

## ✨ Key Features

*   **⚡ Ultra-Low Latency**: Built with **DirectX 11** and **Direct2D** for sub-millisecond rendering.
*   **📉 Minimal Footprint**: Optimized `FramePacer` engine ensures **<2% CPU usage** even on older hardware.
*   **🔒 Safety First**: Relies on standard Windows APIs (`Dwmapi.dll`, `IpHlpApi`) instead of invasive hooks.
*   **🎨 Customizable UI**:
    *   Dark Mode Settings Dashboard (WPF).
    *   Changeable Accent Colors.
    *   Adjustable Target FPS (30, 60, 144, etc.).
*   **⌨️ Global Hotkeys**: Toggle visibility instantly with `Ctrl + Shift + O`.
*   **🔌 System Tray Integration**: unobtrusive background operation.

---

## 🚀 Installation

### Option A: Download EXE
1.  Go to the [**Releases**](https://github.com/suryaprabhaz/fps-overlay/releases) page.
2.  Download the latest `SuryaHUD.exe`.
3.  Run the application.

### Option B: Build from Source
If you prefer to compile it yourself:

1.  **Prerequisites**: Install the [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
2.  **Clone the Repo**:
    ```bash
    git clone https://github.com/suryaprabhaz/fps-overlay.git
    cd fps-overlay
    ```
3.  **Build**:
    Run the included build script:
    ```cmd
    publish.bat
    ```
4.  **Run**:
    The output file will be in `Build/Release/SuryaHUD.exe`.

---

## ⚙️ Configuration

1.  **Open Settings**: Right-click the **System Tray Icon** and select `Settings`.
2.  **Ping Host**: Enter the IP or Domain you want to monitor (e.g., `8.8.8.8` for Google, `1.1.1.1` for Cloudflare).
3.  **Target FPS**: Set the maximum refresh rate for the overlay (e.g., `60`). Lower values save battery on laptops.
4.  **Accent Color**: customized the look with hex codes (e.g., `#FF4500`).

---

## 🛠️ Tech Stack

*   **Language**: C# (.NET 8.0)
*   **Rendering**: Vortice.Windows (Direct3D 11, Direct2D1, DirectWrite)
*   **UI**: WPF (Settings), Win32 API (Overlay Window)
*   **Architecture**: Hybrid Multi-Threaded (Independent Render Thread + UI Thread)

---

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to submit pull requests, report issues, and request features.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Surya Prabhas**
*   GitHub: [@suryaprabhaz](https://github.com/suryaprabhaz)

*Built with passion for high-performance gaming tools.*
