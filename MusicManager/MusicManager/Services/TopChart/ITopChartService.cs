using MusicManager.Models;

namespace MusicManager.Services.TopChart
{
    public interface ITopChartService
    {
        Task<List<TopChartArtist>> TopChartArt_Quarter(int quarter, int quarterYear, int pageSize = 5);
        Task<List<TopChartArtist>> TopChartArt_Year(int year, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Quarter(int quarter, int quarterYear, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Year(int year, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Quarter_Singer(int quarter, int quarterYear, string artistName, string revenuePercentage, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Year_Singer(int year, string artistName, string revenuePercentage, int pageSize = 5);
    }
}
 