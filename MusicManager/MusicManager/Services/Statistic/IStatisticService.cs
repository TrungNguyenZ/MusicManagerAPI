using MusicManager.Models;
using System.Threading.Tasks;

namespace MusicManager.Services
{
    public interface IStatisticService
    {
        Task<StatisticTotalModel> GetTotal(int quarter, int quarterYear);
        Task<StatisticTotalModel> GetTotalBySinger(int quarter, int quarterYear, string artistName, double revenuePercentage);
        Task<List<DigitalMonthSumModel>> Digital_Month_Sum(int year);
        Task<List<DigitalMonthSumModel>> Digital_Month_Sum_Singer(int year, string artistName);
        Task<List<DigitalQuarterPercentModel>> Digital_Quarter_Percent_Singer(int quarter, int quarterYear, string artistName);
        Task<List<DigitalQuarterPercentModel>> Digital_Quarter_Percent(int quarter, int quarterYear);
        Task<List<DigitalYearPercentModel>> Digital_year_Percent(int quarterYear);
        Task<List<DigitalYearPercentModel>> Digital_Year_Percent_Singer(int quarterYear, string artistName);
        Task<List<DigitalQuarterSumModel>> Digital_Quarter_Sum(int year);
        Task<List<DigitalQuarterSumModel>> Digital_Quarter_Sum_Singer(int year, string artistName);
        Task<List<DigitalYearSumModel>> Digital_Year_Sum();
        Task<List<DigitalYearSumModel>> Digital_Year_Sum_Singer(string artistName);
        Task<List<CountryPercentModel>> Country_Quarter_Percent(int quarter, int quarterYear);
        Task<List<CountryPercentModel>> Country_Quarter_Percent_Singer(int quarter, int quarterYear, string artistName);
        Task<List<CountryPercentModel>> Country_Year_Percent(int quarteryear);
        Task<List<CountryPercentModel>> Country_Year_Percent_Singer(int quarteryear, string artistName);
        Task<List<StatisticYoutubeModel>> YoutubeYear(int quarteryear);
        Task<List<StatisticYoutubeModel>> YoutubeYear_Singer(int quarteryear, string artistName);
        Task<List<StatisticYoutubeModel>> YoutubeQuarter(int quarteryear, int quarter);
        Task<List<StatisticYoutubeModel>> YoutubeQuarter_Singer(int quarteryear, int quarter, string artistName);
        Task<List<StatisticPriceNameModel>> PriceYear(int quarteryear);
        Task<List<StatisticPriceNameModel>> PriceYear_Singer(int quarteryear, string artistName);
        Task<List<StatisticPriceNameModel>> PriceQuarter(int quarteryear, int quarter);
        Task<List<StatisticPriceNameModel>> PriceQuarter_Singer(int quarteryear, int quarter, string artistName);
        Task<StatisticTop> StatisticTop_Singer(int type, int quarteryear, int year, string artistName, double revenuePercentage);
        Task<StatisticTop> StatisticTop(int type, int quarteryear, int year);

    }
}
 