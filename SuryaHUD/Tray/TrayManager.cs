using System;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;
using SuryaHUD.UI;
using Microsoft.Extensions.DependencyInjection;

namespace SuryaHUD.Tray
{
    public class TrayManager : IDisposable
    {
        private NotifyIcon _notifyIcon = null!;
        private readonly IServiceProvider _serviceProvider;

        public TrayManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Use default app icon or custom
                Visible = true,
                Text = "SuryaHUD - Made by @SuryaPrabhas"
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Settings", null, (s, e) => OpenSettings());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, (s, e) => ExitApp());

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => OpenSettings();
        }

        private void OpenSettings()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var settingsWindow = _serviceProvider.GetRequiredService<SettingsWindow>();
                settingsWindow.Show();
                settingsWindow.Activate();
            });
        }

        private void ExitApp()
        {
            _notifyIcon.Visible = false;
            Application.Current.Shutdown();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
        }
    }
}
