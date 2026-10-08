using SuryaHUD.Core;
using Xunit;

namespace SuryaHUD.Tests
{
    public class ConfigTests
    {
        [Fact]
        public void DefaultConfig_HasCorrectDefaults()
        {
            var service = new ConfigService();
            Assert.Equal("8.8.8.8", service.Config.PingHost);
            Assert.Equal(60, service.Config.TargetFps);
        }

        [Fact]
        public void Load_DoesNotCrash_OnMissingFile()
        {
            if (System.IO.File.Exists("settings.json"))
                System.IO.File.Delete("settings.json");

            var service = new ConfigService();
            Assert.NotNull(service.Config);
        }
    }
}
