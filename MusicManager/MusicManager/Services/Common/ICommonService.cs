using MusicManager.Models;

namespace MusicManager.Services
{
    public interface ICommonService
    {
        decimal ConvertDecimal(String input);
        long GetNetSinger(double revenuePercentage, double value);
        long GetNetSinger(string revenuePercentage, double value);
        long GetNetSinger(double revenuePercentage, long value);
        long GetNetSinger(string revenuePercentage, long value);
    }
}
 