using System.Threading.Tasks;

namespace SuryaHUD.Core
{
    public interface IPingService
    {
        void Initialize(string hostAddress);
        Task UpdateAsync();
        int GetAveragePing();
        bool IsNetworkAvailable { get; }
        string CurrentStatus { get; } // "Online", "Timeout", "Down"
    }
}
