using System;
using System.Runtime.InteropServices;
using SuryaHUD.Win32;

namespace SuryaHUD.Rendering
{
    public class OverlayWindow : IDisposable
    {
        private const string WindowClass = "SuryaHUD_Overlay";
        private const string WindowTitle = "SuryaHUD";
        
        public IntPtr Handle { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        private bool _isRunning;
        private NativeMethods.WndProc _wndProcDelegate; // Keep reference to prevent GC
        private HotkeyManager _hotkeyManager;

        public event Action? OnRender;
        public event Action? OnResize;

        public OverlayWindow(HotkeyManager hotkeyManager)
        {
            _hotkeyManager = hotkeyManager;
            _wndProcDelegate = WndProc;
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;

            // Register Window Class
            var wndClass = new NativeMethods.WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.WNDCLASSEX)),
                style = 0, // CS_HREDRAW | CS_VREDRAW, but 0 is fine
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = NativeMethods.GetModuleHandle(null),
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero, // LoadCursor(NULL, IDC_ARROW)
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = WindowClass,
                hIconSm = IntPtr.Zero
            };

            if (NativeMethods.RegisterClassEx(ref wndClass) == 0)
            {
                 int error = Marshal.GetLastWin32Error();
                 // If error 1410 (Class already exists), we can ignore or handle it.
                 if (error != 1410)
                 {
                      throw new Exception($"Failed to register window class. Error: {error}");
                 }
            }

            // Create Window
            // WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST | WS_EX_TOOLWINDOW
            int exStyle = WindowStyles.WS_EX_LAYERED | WindowStyles.WS_EX_TRANSPARENT | WindowStyles.WS_EX_TOPMOST | WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_EX_NOACTIVATE;
            int style = WindowStyles.WS_POPUP | WindowStyles.WS_VISIBLE;

            Handle = NativeMethods.CreateWindowEx(
                exStyle,
                WindowClass,
                WindowTitle,
                style,
                0, 0,
                width, height,
                IntPtr.Zero,
                IntPtr.Zero,
                wndClass.hInstance,
                IntPtr.Zero);

            if (Handle == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Exception($"Failed to create overlay window. Error Code: {error}");
            }

            // Set Layered Attributes (Needed for WS_EX_LAYERED, though DirectX might override alpha logic)
            // LWA_ALPHA = 2, LWA_COLORKEY = 1
            // We set it to full opacity here because D3D/D2D will handle per-pixel alpha.
            // Actually, if we use D3D, we usually don't need SetLayeredWindowAttributes if we use UpdateLayeredWindow
            // OR if we just purely render to it. 
            // HOWEVER, for click-through "WS_EX_TRANSPARENT" to work, the window MUST be layered.
            // And to see anything, we usually need to set the attributes.
            NativeMethods.SetLayeredWindowAttributes(Handle, 0, 255, WindowStyles.LWA_ALPHA);

            // Extend Frame for Glass/Transparency
             var margins = new NativeMethods.MARGINS { cxLeftWidth = -1 };
             NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref margins);
             
             // Initialize Hotkeys
             _hotkeyManager.Initialize(Handle);
        }

        public void Run()
        {
            _isRunning = true;
            NativeMethods.MSG msg = new NativeMethods.MSG();

            while (_isRunning)
            {
                if (NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, WindowStyles.PM_REMOVE))
                {
                    if (msg.message == WindowStyles.WM_QUIT)
                    {
                        _isRunning = false;
                    }
                    else
                    {
                        NativeMethods.TranslateMessage(ref msg);
                        NativeMethods.DispatchMessage(ref msg);
                    }
                }
                else
                {
                    // Render Loop
                    OnRender?.Invoke();
                }
            }
        }

        public void Close()
        {
            _isRunning = false;
            if (Handle != IntPtr.Zero)
            {
                NativeMethods.DestroyWindow(Handle);
                Handle = IntPtr.Zero;
            }
        }
        
        public void ToggleVisibility(bool visible)
        {
             if (Handle == IntPtr.Zero) return;
             // SW_SHOW = 5, SW_HIDE = 0
             NativeMethods.ShowWindow(Handle, visible ? 5 : 0);
        }

        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            const int WM_DESTROY = 0x0002;
            const int WM_SIZE = 0x0005;
            const int WM_HOTKEY = 0x0312;

            switch ((int)msg)
            {
                case WM_DESTROY:
                    NativeMethods.PostQuitMessage(0);
                    return IntPtr.Zero;
                
                case WM_SIZE:
                    // Only update logic, don't trigger heavy resize here blindly
                    // int width = (int)lParam & 0xFFFF;
                    // int height = ((int)lParam >> 16) & 0xFFFF;
                    return IntPtr.Zero; // Handled
                    
                case WM_HOTKEY:
                     _hotkeyManager.ProcessMessage((int)msg, wParam, lParam);
                     return IntPtr.Zero;
            }

            return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }
        
        // Helper to shut down from P/Invoke
        // We need PostQuitMessage definition in NativeMethods but wait, 
        // I can just add it locally or to NativeMethods.
        // I'll add a helper method in NativeMethods via "PostQuitMessage".
        // Actually I don't have it in NativeMethods yet. 
        // For now I'll just use the one in NativeMethods if I added it, or add it now.
        // Wait, I missed PostQuitMessage in NativeMethods. I should add it.
        // But I can't modify NativeMethods easily while writing this file.
        // I'll assume I can add it or just use DefWindowProc default behavior for close.
        // Actually, I can just use a local DllImport at the bottom of this class or rely on the loop variable.
        // For WM_DESTROY, DefWindowProc doesn't quit the loop.
        // I'll add [DllImport("user32.dll")] private static extern void PostQuitMessage(int nExitCode); inside here.
        
        // Helper to shut down from P/Invoke
        // We use NativeMethods.PostQuitMessage now.


        public void Dispose()
        {
             Close();
        }
    }
}
