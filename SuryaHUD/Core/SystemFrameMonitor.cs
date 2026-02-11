using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SuryaHUD.Core
{
    public class SystemFrameMonitor : ISystemFrameMonitor
    {
        private readonly Stopwatch _stopwatch;
        private int _frameCount;
        private double _elapsedTime;
        private int _currentRenderFps;

        public SystemFrameMonitor()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void TickRender()
        {
            _frameCount++;
            double seconds = _stopwatch.Elapsed.TotalSeconds;
            if (seconds - _elapsedTime >= 1.0)
            {
                _currentRenderFps = _frameCount;
                _frameCount = 0;
                _elapsedTime = seconds;
            }
        }

        public int GetRenderFps()
        {
            return _currentRenderFps;
        }

        public int GetSystemCompositorRate()
        {
            // Try to get DWM timing info
            try
            {
                DWM_TIMING_INFO timingInfo = new DWM_TIMING_INFO();
                timingInfo.cbSize = Marshal.SizeOf(typeof(DWM_TIMING_INFO));
                
                // P/Invoke call
                // Note: We need to define the P/Invoke or use a library. 
                // For simplicity in this Core class, we might want to abstract the P/Invoke 
                // to the Win32 layer, but for now putting a placeholder or simple logic.
                
                // If DwmGetCompositionTimingInfo is too complex to wire up immediately without 
                // the NativeMethods class ready, we can return a safe default like 60 
                // or try to P/Invoke if NativeMethods is ready.
                
                // Let's assume NativeMethods will expose this.
                // For now, return a placeholder or 60 to prevent build errors until Win32 is ready.
                return 60; 
            }
            catch
            {
                return 60;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DWM_TIMING_INFO
        {
            public int cbSize;
            public ulong rateRefresh;
            public ulong qpcRefreshPeriod;
            public ulong rateCompose;
            public ulong qpcVBlank;
            public ulong cRefresh;
            public uint cDXRefresh;
            public ulong qpcCompose;
            public ulong cFrame;
            public uint cDXPresent;
            public ulong cRefreshFrame;
            public ulong cFrameSubmitted;
            public uint cDXPresentSubmitted;
            public ulong cFrameConfirmed;
            public uint cDXPresentConfirmed;
            public ulong cRefreshConfirmed;
            public uint cDXRefreshConfirmed;
            public ulong cFramesLate;
            public uint cFramesOutstanding;
            public ulong cFrameDisplayed;
            public ulong qpcFrameDisplayed;
            public ulong cRefreshFrameDisplayed;
            public ulong cFrameComplete;
            public ulong qpcFrameComplete;
            public ulong cFramePending;
            public ulong qpcFramePending;
            public ulong cFramesDisplayed;
            public ulong cFramesComplete;
            public ulong cFramesPending;
            public ulong cFramesAvailable;
            public ulong cFramesDropped;
            public ulong cFramesMissed;
            public ulong cRefreshNextDisplayed;
            public ulong cRefreshNextPresented;
            public ulong cRefreshesDisplayed;
            public ulong cRefreshesPresented;
            public ulong cRefreshStarted;
            public ulong cPixelsReceived;
            public ulong cPixelsDrawn;
            public ulong cBuffersEmpty;
        }
    }
}
