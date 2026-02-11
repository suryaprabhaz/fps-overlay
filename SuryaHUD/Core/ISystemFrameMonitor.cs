namespace SuryaHUD.Core
{
    public interface ISystemFrameMonitor
    {
        void TickRender(); // Called every render frame
        int GetRenderFps();
        int GetSystemCompositorRate(); // DWM rate
    }
}
