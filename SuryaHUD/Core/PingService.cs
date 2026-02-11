using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace SuryaHUD.Core
{
    public class PingService : IPingService
    {
        private string _hostAddress = "8.8.8.8";
        private readonly IPingSender _pingSender;
        private readonly Queue<long> _pingHistory;
        private const int HistorySize = 10;
        private const int TimeoutMs = 2000;

        public bool IsNetworkAvailable { get; private set; } = true;
        public string CurrentStatus { get; private set; } = "Initializing";

        public PingService(IPingSender? pingSender = null)
        {
            _pingSender = pingSender ?? new SystemPingSender();
            _pingHistory = new Queue<long>(HistorySize);
        }

        public void Initialize(string hostAddress)
        {
            // Simple validation could be added here
            if (!string.IsNullOrWhiteSpace(hostAddress))
            {
                _hostAddress = hostAddress;
            }
        }

        public async Task UpdateAsync()
        {
            try
            {
                PingReplyWrapper reply = await _pingSender.SendPingAsync(_hostAddress, TimeoutMs);

                if (reply.Status == IPStatus.Success)
                {
                    IsNetworkAvailable = true;
                    CurrentStatus = "Online";
                    AddPing(reply.RoundtripTime);
                }
                else if (reply.Status == IPStatus.TimedOut)
                {
                    IsNetworkAvailable = true; // Network might be up, but target is slow
                    CurrentStatus = "Timeout";
                    // Optionally add a penalty ping or just ignore
                    // AddPing(TimeoutMs); // Penalize? Or just don't add? 
                    // Let's not add timeouts to the average to keep the "live" latency accurate, 
                    // but the status will reflect the issue.
                }
                else
                {
                    IsNetworkAvailable = false;
                    CurrentStatus = "Down";
                }
            }
            catch (PingException)
            {
                IsNetworkAvailable = false;
                CurrentStatus = "Error";
            }
            catch (Exception)
            {
                IsNetworkAvailable = false;
                CurrentStatus = "Error";
            }
        }
        // Removed SendPingInternal as we use IPingSender now

        private void AddPing(long ping)
        {
            if (_pingHistory.Count >= HistorySize)
            {
                _pingHistory.Dequeue();
            }
            _pingHistory.Enqueue(ping);
        }

        public int GetAveragePing()
        {
            if (_pingHistory.Count == 0) return 0;
            return (int)_pingHistory.Average();
        }
    }
}
