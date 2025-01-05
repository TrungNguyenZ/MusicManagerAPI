using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;
using System.Linq;

namespace MusicManager.Repositories
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly AppDbContext _context;

        public StatisticRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StatisticTotalModel> GetTotal(int quarter, int quarterYear)
        {
            var result = await _context.StatisticTotalModels
                .FromSqlRaw("EXEC SP_netIncome_total @quarter = {0}, @quarterYear = {1}", quarter, quarterYear)
                .ToListAsync(); 

            return result.FirstOrDefault();
        }
        public async Task<StatisticTotalModel> GetTotalBySinger(int quarter, int quarterYear, string artistName)
        {
            var result = await _context.StatisticTotalModels
                .FromSqlRaw("EXEC SP_netIncome_total_singer @quarter = {0}, @quarterYear = {1} , @artistName = {2}", quarter, quarterYear, artistName)
                .ToListAsync();

            return result.FirstOrDefault();
        }
        public async Task<List<DigitalMonthSumModel>> Digital_Month_Sum(int year)
        {
            var result = await _context.DigitalMonthSumModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_month_sum @year = {0}", year)
                .ToListAsync();

            return result;
        }
        public async Task<List<DigitalMonthSumModel>> Digital_Month_Sum_Singer(int year, string artisName)
        {
            var result = await _context.DigitalMonthSumModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_month_sum_singer @year = {0} , @artistName = {1}", year, artisName)
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalQuarterPercentModel>> Digital_Quarter_Percent_Singer(int quarter, int quarterYear, string artisName)
        {
            var result = await _context.DigitalQuarterPercentModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_quarter_percent_singer @quarterYear = {0}, @quarter = {1} , @artistName = {2}", quarterYear, quarter , artisName)
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalQuarterPercentModel>> Digital_Quarter_Percent(int quarter, int quarterYear)
        {
            var result = await _context.DigitalQuarterPercentModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_quarter_percent @quarterYear = {0}, @quarter = {1}", quarterYear, quarter)
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalYearPercentModel>> Digital_Year_Percent_Singer(int quarterYear, string artisName)
        {
            var result = await _context.DigitalYearPercentModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_year_percent_singer @quarterYear = {0}", quarterYear, artisName)
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalYearPercentModel>> Digital_year_Percent(int quarterYear)
        {
            var result = await _context.DigitalYearPercentModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_year_percent @quarterYear = {0}", quarterYear)
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalQuarterSumModel>> Digital_Quarter_Sum(int year)
        {
            var result = await _context.DigitalQuarterSumModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_quarter_sum @year = {0}", year)
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalQuarterSumModel>> Digital_Quarter_Sum_Singer(int year, string artisName)
        {
            var result = await _context.DigitalQuarterSumModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_quarter_sum_singer @year = {0} , @artistName = {1}", year, artisName)
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalYearSumModel>>Digital_Year_Sum()
        {
            var result = await _context.DigitalYearSumModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_year_sum")
                .ToListAsync();
            return result;
        }
        public async Task<List<DigitalYearSumModel>> Digital_Year_Sum_Singer(string artisName)
        {
            var result = await _context.DigitalYearSumModel
                .FromSqlRaw("EXEC SP_statistic_digitalService_year_sum_singer @artistName = {0}", artisName)
                .ToListAsync();
            return result;
        }
        public async Task<List<CountryPercentModel>> Country_Quarter_Percent(int quarter, int quarterYear)
        {
            var result = await _context.CountryPercentModel
                .FromSqlRaw("EXEC SP_statistic_country_quarter_percent @quarterYear = {0} , @quarter = {1}", quarterYear, quarter)
                .ToListAsync();
            return result;
        }
        public async Task<List<CountryPercentModel>> Country_Quarter_Percent_Singer(int quarter, int quarterYear, string artisName)
        {
            var result = await _context.CountryPercentModel
                .FromSqlRaw("EXEC SP_statistic_country_quarter_percent_singer @quarterYear = {0} , @quarter = {1} , @artisName = {2}", quarterYear, quarter, artisName)
                .ToListAsync();
            return result;
        }

        public async Task<List<CountryPercentModel>> Country_Year_Percent(int quarteryear)
        {
            var result = await _context.CountryPercentModel
                .FromSqlRaw("EXEC SP_statistic_country_year_percent @quarterYear = {0}", quarteryear)
                .ToListAsync();
            return result;
        }
        public async Task<List<CountryPercentModel>> Country_Year_Percent_Singer(int quarteryear, string artisName)
        {
            var result = await _context.CountryPercentModel
                .FromSqlRaw("EXEC SP_statistic_country_year_percent_singer @quarterYear = {0} , @artistName = {1}", quarteryear, artisName)
                .ToListAsync();
            return result;
        }   
        public async Task<List<StatisticYoutubeModel>> YoutubeQuarter(int quarteryear, int quarter)
        {
            var result = await _context.StatisticYoutubeModel
                .FromSqlRaw("EXEC SP_statistic_youtube_quarter_pecent @quarterYear = {0} , @quarter = {1}", quarteryear, quarter)
                .ToListAsync();
            return result;
        }
        public async Task<List<StatisticYoutubeModel>> YoutubeQuarter_Singer(int quarteryear, int quarter, string artistName)
        {
            var result = await _context.StatisticYoutubeModel
                .FromSqlRaw("EXEC SP_statistic_youtube_quarter_pecent_singer @quarterYear = {0} , @quarter = {1}, @artistName", quarteryear, quarter, artistName)
                .ToListAsync();
            return result;
        }
        public async Task<List<StatisticYoutubeModel>> YoutubeYear(int quarteryear)
        {
            var result = await _context.StatisticYoutubeModel
                .FromSqlRaw("EXEC SP_statistic_youtube_year_pecent @quarterYear = {0}", quarteryear)
                .ToListAsync();
            return result;
        }
        public async Task<List<StatisticYoutubeModel>> YoutubeYear_Singer(int quarteryear, string artistName)
        {
            var result = await _context.StatisticYoutubeModel
                .FromSqlRaw("EXEC SP_statistic_youtube_year_pecent_singer @quarterYear = {0} , @artistName = {1}", quarteryear, artistName)
                .ToListAsync();
            return result;
        }
        public async Task<List<StatisticPriceNameModel>> PriceYear(int quarteryear)
        {
            var result = await _context.StatisticPriceNameModel
                .FromSqlRaw("EXEC SP_statistic_priceName_year_pecent @quarterYear = {0}", quarteryear)
                .ToListAsync();
            return result;
        }
        public async Task<List<StatisticPriceNameModel>> PriceYear_Singer(int quarteryear, string artistName)
        {
            var result = await _context.StatisticPriceNameModel
                .FromSqlRaw("EXEC SP_statistic_priceName_year_pecent_singer @quarterYear = {0} , @artistName = {1}", quarteryear, artistName)
                .ToListAsync();
            return result;
        }
        public async Task<List<StatisticPriceNameModel>> PriceQuarter(int quarteryear, int quarter)
        {
            var result = await _context.StatisticPriceNameModel
                .FromSqlRaw("EXEC SP_statistic_priceName_quarter_pecent @quarterYear = {0} , @quarter = {1}", quarteryear, quarter)
                .ToListAsync();
            return result;
        }
        public async Task<List<StatisticPriceNameModel>> PriceQuarter_Singer(int quarteryear, int quarter, string artistName)
        {
            var result = await _context.StatisticPriceNameModel
                .FromSqlRaw("EXEC SP_statistic_priceName_quarter_pecent_singer @quarterYear = {0} , @quarter = {1}, @artistName = {2}", quarteryear, quarter, artistName)
                .ToListAsync();
            return result;
        }
    }
}
