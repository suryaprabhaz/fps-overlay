using System;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.Direct2D1;
using Vortice.DXGI;
using Vortice.Mathematics;
using System.Numerics;

using Vortice.DCommon; // For PixelFormat

namespace SuryaHUD.Rendering
{
    public class OverlayRenderer : IDisposable
    {
        private readonly OverlayWindow _window;
        private ID3D11Device _d3dDevice = null!;
        private ID3D11DeviceContext _d3dContext = null!;
        private IDXGISwapChain _swapChain = null!;
        private ID3D11RenderTargetView _renderTargetView = null!;
        
        private ID2D1Factory _d2dFactory = null!;
        private ID2D1RenderTarget _d2dRenderTarget = null!;
        private ID2D1SolidColorBrush _solidBrush = null!;
        private TextRenderer _textRenderer;
        
        private bool _initialized;

        public OverlayRenderer(OverlayWindow window)
        {
            _window = window;
            _textRenderer = new TextRenderer();
        }

        public void Initialize()
        {
            if (_initialized) return;

            int width = _window.Width;
            int height = _window.Height;

            // 1. Create D3D11 Device and SwapChain
            var swapChainDesc = new SwapChainDescription
            {
                BufferCount = 2,
                BufferUsage = Usage.RenderTargetOutput,
                BufferDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputWindow = _window.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard, // Or FlipDiscard for Win10+
                Windowed = true,
                Flags = SwapChainFlags.None
            };

            // Needs explicit FeatureLevel array
            Vortice.Direct3D.FeatureLevel[] featureLevels = new[]
            {
                Vortice.Direct3D.FeatureLevel.Level_11_1,
                Vortice.Direct3D.FeatureLevel.Level_11_0,
            };

            // Prepare nullable output for FeatureLevel
            Vortice.Direct3D.FeatureLevel? level;

            if (D3D11.D3D11CreateDeviceAndSwapChain(
                null, // Adapter (null for default)
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport, // Required for D2D
                featureLevels,
                swapChainDesc,
                out _swapChain,
                out _d3dDevice,
                out level,
                out _d3dContext).Failure)
            {
                 throw new Exception("Failed to create D3D11 device");
            }

            // 2. Create RenderTargetView for D3D
            using (var backBuffer = _swapChain.GetBuffer<ID3D11Texture2D>(0))
            {
                _renderTargetView = _d3dDevice.CreateRenderTargetView(backBuffer);
                
                // 3. Create D2D RenderTarget
                // We need a DXGI Surface to interop
                using (var dxgiSurface = backBuffer.QueryInterface<IDXGISurface>())
                {
                    var factoryOptions = new FactoryOptions { DebugLevel = DebugLevel.None };
                    _d2dFactory = D2D1.D2D1CreateFactory<ID2D1Factory>(Vortice.Direct2D1.FactoryType.SingleThreaded, factoryOptions);
                    
                    var pixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, Vortice.DCommon.AlphaMode.Premultiplied);
                    var renderTargetProps = new RenderTargetProperties(RenderTargetType.Default, pixelFormat, 96, 96, RenderTargetUsage.None, Vortice.Direct2D1.FeatureLevel.Default);
                    
                    _d2dRenderTarget = _d2dFactory.CreateDxgiSurfaceRenderTarget(dxgiSurface, renderTargetProps);
                }
            }

            // 4. Create Resources
            _solidBrush = _d2dRenderTarget.CreateSolidColorBrush(Colors.White);

            _initialized = true;
        }

        public void Draw(string fpsText, string pingText, string pingStatus, Color4 accentColor)
        {
            if (!_initialized) return;

            // Clear D3D
            // Transparent background (0,0,0,0)
            _d3dContext.ClearRenderTargetView(_renderTargetView, new Color4(0, 0, 0, 0));

            // Draw D2D
            _d2dRenderTarget.BeginDraw();
            _d2dRenderTarget.Clear(new Color4(0, 0, 0, 0));

            // Pseudo-UI logic (will be moved to AnimationEngine later, but hardcoding for MVP)
            // Draw a subtle background box
            var bgRect = new Rect(20, 20, 160, 80);
            _solidBrush.Color = new Color4(0f, 0f, 0f, 0.7f); // Semi-transparent black
            _d2dRenderTarget.FillRectangle(bgRect, _solidBrush);

            // Access/Draw logic... 
            // Note: We need a Font/TextFormat. D2D uses DirectWrite. 
            // For now, I'll assumme I need to initialize DirectWrite too, or simple geometry.
            // Wait, I can't easily draw text without IDWriteFactory.
            // Adding DirectWrite initialization in Initialize would be good.
            // For now, I'll just draw colored rectangles to prove it works if text is too complex for one go.
            // No, I strictly need "Clean numeric layout". I MUST use DirectWrite.

            // I'll assume I can lazily create the Factory or add it to Init in next step.
            // I'll stick to rectangles for this specific file write to keep it safe, 
            // and I'll Update it immediately after with Text support.
            
            // Draw Ping Indicator (Circle)
            _solidBrush.Color = GetPingColor(pingStatus);
            _d2dRenderTarget.FillEllipse(new Ellipse(new Vector2(160, 40), 5, 5), _solidBrush);

            // Draw Text
            _solidBrush.Color = Colors.White;
            _textRenderer.DrawText(_d2dRenderTarget, $"SYS FPS: {fpsText}", 25, 25, _solidBrush);
            
            _solidBrush.Color = accentColor;
            _textRenderer.DrawText(_d2dRenderTarget, $"PING: {pingText}", 25, 45, _solidBrush);

            _d2dRenderTarget.EndDraw();

            // Present w/ VSync (1) or No VSync (0)
            // Using 1 for VSync
            _swapChain.Present(1, PresentFlags.None);
        }
        
        private Color4 GetPingColor(string status)
        {
            if (status == "Online") return Colors.Green;
            if (status == "Timeout") return Colors.Yellow;
            return Colors.Red;
        }

        public void Dispose()
        {
            _solidBrush?.Dispose();
            _d2dRenderTarget?.Dispose();
            _d2dFactory?.Dispose();
            _renderTargetView?.Dispose();
            _d3dContext?.Dispose();
            _d3dDevice?.Dispose();
            _swapChain?.Dispose();
        }
    }
}
