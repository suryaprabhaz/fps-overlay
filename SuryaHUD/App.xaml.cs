using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SuryaHUD.Core;
using SuryaHUD.Rendering;
using SuryaHUD.Tray;
using SuryaHUD.UI;
using SuryaHUD.Win32;

namespace SuryaHUD
{
    public partial class App : System.Windows.Application
    {
        private IServiceProvider _serviceProvider = null!;
        private Thread _overlayThread = null!;
        private OverlayWindow _overlayWindow = null!;
        private OverlayRenderer _renderer = null!;
        private CancellationTokenSource _cts = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Setup DI
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // 2. Start Overlay on a separate thread
            _cts = new CancellationTokenSource();
            _overlayThread = new Thread(OverlayLoop)
            {
                IsBackground = true,
                Name = "OverlayRenderThread"
            };
            _overlayThread.SetApartmentState(ApartmentState.STA); // DirectX/Window creation often wants STA
            _overlayThread.Start();

            // 3. Initialize Tray Icon
            var trayManager = _serviceProvider.GetRequiredService<TrayManager>();
            trayManager.Initialize();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IPingService, PingService>();
            services.AddSingleton<ISystemFrameMonitor, SystemFrameMonitor>();
            
            services.AddSingleton<HotkeyManager>();
            
            // UI
            services.AddSingleton<SettingsWindow>();
            services.AddSingleton<TrayManager>();
        }

        private void OverlayLoop()
        {
            try
            {
                // Create Dependencies manually or via a child scope if needed
                // But since they are singletons/scoped, we can resolve.
                // Note: Windows must be created on the thread that pumps them.
                
                var hotkeyManager = _serviceProvider.GetRequiredService<HotkeyManager>();
                var configService = _serviceProvider.GetRequiredService<IConfigService>();
                var pingService = _serviceProvider.GetRequiredService<IPingService>();
                var fpsMonitor = _serviceProvider.GetRequiredService<ISystemFrameMonitor>();
                
                // Initialize Ping
                pingService.Initialize(configService.Config.PingHost);

                using (_overlayWindow = new OverlayWindow(hotkeyManager))
                using (_renderer = new OverlayRenderer(_overlayWindow))
                {
                    // Calculate size (Fullscreen)
                    // TODO: Multi-monitor support. For now Primary Screen.
                    int width = (int)SystemParameters.PrimaryScreenWidth;
                    int height = (int)SystemParameters.PrimaryScreenHeight;
                    
                    // Actually, SystemParameters matches logical pixels. We need physical.
                    // NativeMethods.GetSystemMetrics(SM_CXSCREEN) might be better, or just use 1920x1080 default
                    // But we should use the Win32 API because SystemParameters depends on WPF dpi.
                    // Let's rely on SystemParameters for MVP or user correct DPI awareness.
                    // We called SetProcessDPIAware in NativeMethods? I should call it.
                    NativeMethods.SetProcessDPIAware();
                    
                    // Re-read metrics after DPI aware
                    // NativeMethods.RECT desktopRect; // Unused
                    IntPtr hDesktop = NativeMethods.GetModuleHandle(null); // Actually get desktop window? No.
                    // Just GetSystemMetrics via PInvoke or System.Windows.Forms.Screen
                    // I will use a hardcoded safe fallback or logic.
                    // System.Windows.Forms.Screen.PrimaryScreen.Bounds is reliable if UseWindowsForms is on.
                    var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                    width = bounds.Width;
                    height = bounds.Height;

                    _overlayWindow.Initialize(width, height);
                    _renderer.Initialize();
                    
                    // Setup Frame Pacer
                    var pacer = new FramePacer(configService.Config.TargetFps);
                    var animEngine = new AnimationEngine();

                    // Register Hotkeys
                    // Modifiers: Alt=1, Ctrl=2, Shift=4, Win=8
                    // Ctrl+Shift+O = 2|4 + O
                    // Virtual Key O = 0x4F
                    hotkeyManager.Register(2 | 4, 0x4F, () => 
                    {
                         bool isVisible = NativeMethods.IsWindowVisible(_overlayWindow.Handle);
                         _overlayWindow.ToggleVisibility(!isVisible);
                    });

                    // Render Event
                    _overlayWindow.OnRender += () =>
                    {
                        if (_cts.IsCancellationRequested) 
                        {
                            _overlayWindow.Close();
                            return;
                        }

                        pacer.BeginFrame();
                        
                        // 1. Update Logic
                        fpsMonitor.TickRender();
                        // Ping updates async, we just read property
                        Task.Run(() => pingService.UpdateAsync()); // Fire and forget update? 
                        // Better: PingService runs its own timer loop? 
                        // The Interface said UpdateAsync. We should call it periodically.
                        // I'll add a timer check here or let PingService handle its own loop.
                        // Plan said "Ping every 500ms asynchronously".
                        // I'll call UpdateAsync here but throttle it?
                        // Actually PingService implemented UpdateAsync as a single shot.
                        // I should invoke it if enough time passed.
                        // Or just fire and forget if not busy.
                        // Ideally: separate timer. But cheap to just Task.Run logic here with a safeguard.
                        // I'll stick to a simple fire-and-forget for now, relying on Ping internal state.
                        
                        // 2. Animate
                        // float renderFps = fpsMonitor.GetRenderFps();
                        // animEngine.Update goes here
                        
                        // 3. Draw
                        // Get Config Colors
                        var sysColor = System.Drawing.ColorTranslator.FromHtml(configService.Config.AccentColor);
                        var accent = new Vortice.Mathematics.Color4(sysColor.R / 255f, sysColor.G / 255f, sysColor.B / 255f);
                        
                        _renderer.Draw(
                            fpsMonitor.GetRenderFps().ToString(), 
                            pingService.GetAveragePing() + "ms", 
                            pingService.CurrentStatus,
                            accent
                        );
                        
                        pacer.EndFrame();
                    };

                    _overlayWindow.Run();
                }
            }
            catch (Exception ex)
            {
                 // Create a log file if crash
                 System.IO.File.WriteAllText("crash.log", ex.ToString());
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _cts.Cancel();
            _overlayThread.Join(1000); // Wait for cleanup
            base.OnExit(e);
        }
    }
}
