using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using SuryaHUD.Core;
using Xunit;

namespace SuryaHUD.Tests
{
    public class PingServiceTests
    {
        class MockPingSender : IPingSender
        {
            public IPStatus StatusToReturn { get; set; } = IPStatus.Success;
            public long RttToReturn { get; set; } = 20;

            public Task<PingReplyWrapper> SendPingAsync(string host, int timeout)
            {
                return Task.FromResult(new PingReplyWrapper(StatusToReturn, RttToReturn));
            }
        }

        [Fact]
        public async Task UpdateAsync_Success_ShouldUpdateAverage()
        {
            var mock = new MockPingSender { RttToReturn = 50 };
            var service = new PingService(mock);

            await service.UpdateAsync();
            await service.UpdateAsync();

            Assert.Equal("Online", service.CurrentStatus);
            Assert.Equal(50, service.GetAveragePing());
        }

        [Fact]
        public async Task UpdateAsync_Timeout_ShouldNotBreakAverageButSetStatus()
        {
            var mock = new MockPingSender { RttToReturn = 50 };
            var service = new PingService(mock);

            await service.UpdateAsync(); // 50ms
            
            mock.StatusToReturn = IPStatus.TimedOut;
            await service.UpdateAsync(); 

            Assert.Equal("Timeout", service.CurrentStatus);
            Assert.Equal(50, service.GetAveragePing()); // Should still be 50 if timeout ignored from avg
        }

        [Fact]
        public async Task UpdateAsync_RollingAverage_ShouldWork()
        {
            var mock = new MockPingSender { RttToReturn = 10 };
            var service = new PingService(mock);

            for(int i=0; i<5; i++) await service.UpdateAsync();
            
            mock.RttToReturn = 20;
            for(int i=0; i<5; i++) await service.UpdateAsync();
            
            // 5 * 10 + 5 * 20 = 50 + 100 = 150 / 10 = 15
            Assert.Equal(15, service.GetAveragePing());
        }
    }
}
