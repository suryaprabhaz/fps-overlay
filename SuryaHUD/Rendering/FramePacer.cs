using System;
using System.Diagnostics;
using System.Threading;

namespace SuryaHUD.Rendering
{
    public class FramePacer
    {
        private readonly Stopwatch _stopwatch;
        private double _targetFrameTimeMs; // e.g., 16.66ms for 60fps

        public FramePacer(int targetFps)
        {
            _stopwatch = new Stopwatch();
            SetTargetFps(targetFps);
        }

        public void SetTargetFps(int fps)
        {
            if (fps <= 0) fps = 60;
            _targetFrameTimeMs = 1000.0 / fps;
        }

        public void BeginFrame()
        {
            _stopwatch.Restart();
        }

        public void EndFrame()
        {
            _stopwatch.Stop();
            double elapsedMs = _stopwatch.Elapsed.TotalMilliseconds;
            
            // If we have time left, sleep
            if (elapsedMs < _targetFrameTimeMs)
            {
                int sleepTime = (int)(_targetFrameTimeMs - elapsedMs);
                if (sleepTime > 1)
                {
                    // Basic Sleep is efficient for CPU.
                    // We accept slight jitter (<15ms resolution on some systems) 
                    // in exchange for <2% CPU usage.
                    // "timeBeginPeriod" could improve resolution but increases global timer tick.
                    // For an overlay, standard sleep is usually acceptable if VSync is also on.
                    Thread.Sleep(sleepTime); 
                }
            }
        }
    }
}
