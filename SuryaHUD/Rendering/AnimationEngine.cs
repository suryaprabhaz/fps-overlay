using System;
using Vortice.Mathematics;

namespace SuryaHUD.Rendering
{
    public class AnimationEngine
    {
        // Simple linear interpolation for values
        public static float Lerp(float start, float end, float amount)
        {
            return start + (end - start) * amount;
        }

        public static Color4 LerpColor(Color4 start, Color4 end, float amount)
        {
            return new Color4(
                Lerp(start.R, end.R, amount),
                Lerp(start.G, end.G, amount),
                Lerp(start.B, end.B, amount),
                Lerp(start.A, end.A, amount)
            );
        }

        // State for smoothed values
        private float _currentFps;
        private float _currentPing;
        
        public float SmoothedFps => _currentFps;
        public float SmoothedPing => _currentPing;

        public void Update(float targetFps, float targetPing, float dtSeconds)
        {
            // Speed of interpolation (adjust for snappiness)
            float speed = 10.0f; 
            
            _currentFps = Lerp(_currentFps, targetFps, speed * dtSeconds);
            _currentPing = Lerp(_currentPing, targetPing, speed * dtSeconds);
        }
    }
}
