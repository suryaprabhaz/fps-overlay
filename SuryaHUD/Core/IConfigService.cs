namespace SuryaHUD.Core
{
    public interface IConfigService
    {
        HudConfig Config { get; }
        void Save();
        void Load();
    }

    public class HudConfig
    {
        public string PingHost { get; set; } = "8.8.8.8";
        public int TargetFps { get; set; } = 60;
        public bool ShowFps { get; set; } = true;
        public bool ShowPing { get; set; } = true;
        public string Position { get; set; } = "TopLeft"; // TopLeft, TopRight, BottomLeft, BottomRight
        public string AccentColor { get; set; } = "#00FFFF"; // Cyan
    }
}
