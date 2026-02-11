using System.Windows;
using SuryaHUD.Core;

namespace SuryaHUD.UI
{
    public partial class SettingsWindow : Window
    {
        private readonly IConfigService _configService;

        // Constructor for DI
        public SettingsWindow(IConfigService configService)
        {
            InitializeComponent();
            _configService = configService;
            LoadValues();
        }
        
        // Parameterless constructor for XAML designer (optional, but good practice if needed)
        // public SettingsWindow() { InitializeComponent(); }

        private void LoadValues()
        {
            if (_configService == null) return;
            TxtPingHost.Text = _configService.Config.PingHost;
            TxtTargetFps.Text = _configService.Config.TargetFps.ToString();
            TxtAccentColor.Text = _configService.Config.AccentColor;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _configService.Config.PingHost = TxtPingHost.Text;
            
            if (int.TryParse(TxtTargetFps.Text, out int fps))
            {
                _configService.Config.TargetFps = fps;
            }
            
            _configService.Config.AccentColor = TxtAccentColor.Text;
            
            _configService.Save();
            
            // Ideally notify services to reload config. For MVP, restart required or poll.
            // We can add "MessageBox.Show("Settings Saved. Please restart to apply some changes.");"
            System.Windows.MessageBox.Show("Settings Saved.", "SuryaHUD", MessageBoxButton.OK, MessageBoxImage.Information);
            
            Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
