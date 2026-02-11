using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace SuryaHUD.Core
{
    public interface IPingSender
    {
        Task<PingReplyWrapper> SendPingAsync(string host, int timeout);
    }
    
    public class PingReplyWrapper
    {
        public IPStatus Status { get; set; }
        public long RoundtripTime { get; set; }
        
        public PingReplyWrapper(IPStatus status, long rtt)
        {
            Status = status;
            RoundtripTime = rtt;
        }
    }

    public class SystemPingSender : IPingSender
    {
        private readonly Ping _ping = new Ping();
        
        public async Task<PingReplyWrapper> SendPingAsync(string host, int timeout)
        {
            var reply = await _ping.SendPingAsync(host, timeout);
            return new PingReplyWrapper(reply.Status, reply.RoundtripTime);
        }
    }
}
