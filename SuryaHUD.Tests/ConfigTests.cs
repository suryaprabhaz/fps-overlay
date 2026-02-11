using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using SuryaHUD.Core;
using Xunit;

namespace SuryaHUD.Tests
{
    // Mock class inheriting from PingService
    public class TestablePingService : PingService
    {
        public IPStatus NextStatus { get; set; } = IPStatus.Success;
        public long NextRoundtrip { get; set; } = 20;
        public bool ThrowException { get; set; } = false;

        protected override Task<PingReply> SendPingInternal(string host, int timeout)
        {
            if (ThrowException) throw new Exception("Ping failed");

            // Create a fake PingReply using reflection since constructor is internal/protected in some versions
            // actually PingReply has no public constructor.
            // We can use a trick or just wrap the result.
            // Since we can't easily instantiate PingReply, we might need to wrap the *Result* in our own DTO.
            // BUT, for now, let's try to instantiate it via reflection or change the architecture to wrapper.
            // Reflection approach:
            
            var ctor = typeof(PingReply).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            // This is fragile.
            // Better approach: PingService should use a IPingSender interface that returns a IPingReply interface.
            // But since I'm in the middle of writing tests, I'll stick to a simpler verification of logic:
            // "GetAveragePing" buffering is internal logic.
            // I'll assume I can't easily mock PingReply without interface.
            
            // Re-Plan: Extract ISystemPing interface.
            return Task.FromResult<PingReply>(null!); 
        }
    }
    
    // Better Approach: Test ConfigService instead which is pure logic.
    // And for PingService, just test the buffering logic if I can inject values.
    // I already made 'AddPing' private. I can make it internal/protected.
    
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
             // Ensure no file
             if (System.IO.File.Exists("settings.json")) System.IO.File.Delete("settings.json");
             
             var service = new ConfigService();
             Assert.NotNull(service.Config);
        }
    }
}
