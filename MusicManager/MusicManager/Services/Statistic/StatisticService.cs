using Microsoft.AspNetCore.Mvc;
using MusicManager.Models;
using MusicManager.Repositories;

namespace MusicManager.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repository;
        private readonly IRepository<ApplicationUser> _repositoryUser;
        private readonly ICommonService _commonService;
        public StatisticService(IStatisticRepository repository, IRepository<ApplicationUser> repositoryUser, ICommonService commonService)
        {
            _repository = repository;
            _repositoryUser = repositoryUser;
            _commonService = commonService;
        }
        public async Task<StatisticTotalModel> GetTotal(int quarter, int quarterYear)
        {
            return await _repository.GetTotal(quarter, quarterYear);
        }
        public async Task<StatisticTotalModel> GetTotalBySinger(int quarter, int quarterYear, string artistName, double revenuePercentage)
        {

            var data = await _repository.GetTotalBySinger(quarter, quarterYear, artistName);
            data.TotalForYear = _commonService.GetNetSinger(revenuePercentage, data.TotalForYear);     
            data.TotalForQuarterYear = _commonService.GetNetSinger(revenuePercentage, data.TotalForQuarterYear);   
            data.TotalForAll = _commonService.GetNetSinger(revenuePercentage, data.TotalForAll);
            return data;
        }
        public async Task<List<DigitalMonthSumModel>> Digital_Month_Sum( int year)
        {
            return await _repository.Digital_Month_Sum(year);
        }
        public async Task<List<DigitalMonthSumModel>> Digital_Month_Sum_Singer(int year, string artistName)
        {
            return await _repository.Digital_Month_Sum_Singer(year, artistName);
        }     
        public async Task<List<DigitalQuarterPercentModel>> Digital_Quarter_Percent(int quarter, int quarterYear)
        {
            return await _repository.Digital_Quarter_Percent(quarter, quarterYear);
        }
        public async Task<List<DigitalQuarterPercentModel>> Digital_Quarter_Percent_Singer(int quarter, int quarterYear, string artistName)
        {
            return await _repository.Digital_Quarter_Percent_Singer(quarter, quarterYear, artistName);
        }
        public async Task<List<DigitalYearPercentModel>> Digital_year_Percent(int quarterYear)
        {
            return await _repository.Digital_year_Percent(quarterYear);
        }
        public async Task<List<DigitalYearPercentModel>> Digital_Year_Percent_Singer(int quarterYear, string artistName)
        {
            return await _repository.Digital_Year_Percent_Singer(quarterYear, artistName);
        }    
        public async Task<List<DigitalQuarterSumModel>> Digital_Quarter_Sum(int year)
        {
            return await _repository.Digital_Quarter_Sum(year);
        }
        public async Task<List<DigitalQuarterSumModel>> Digital_Quarter_Sum_Singer(int year, string artistName)
        {
            return await _repository.Digital_Quarter_Sum_Singer(year, artistName);
        }    
        public async Task<List<DigitalYearSumModel>> Digital_Year_Sum()
        {
            return await _repository.Digital_Year_Sum();
        } 
        public async Task<List<DigitalYearSumModel>> Digital_Year_Sum_Singer(string artistName)
        {
            return await _repository.Digital_Year_Sum_Singer(artistName);
        }
        public async Task<List<CountryPercentModel>> Country_Quarter_Percent(int quarter, int quarterYear)
        {
            return await _repository.Country_Quarter_Percent(quarter, quarterYear);
        }
        public async Task<List<CountryPercentModel>> Country_Quarter_Percent_Singer(int quarter, int quarterYear, string artistName)
        {
            return await _repository.Country_Quarter_Percent_Singer(quarter, quarterYear, artistName);
        }   
        public async Task<List<CountryPercentModel>> Country_Year_Percent(int quarteryear)
        {
            return await _repository.Country_Year_Percent(quarteryear);
        }    
        public async Task<List<CountryPercentModel>> Country_Year_Percent_Singer(int quarteryear, string artistName)
        {
            return await _repository.Country_Year_Percent_Singer(quarteryear, artistName);
        }
        public async Task<List<StatisticYoutubeModel>> YoutubeYear(int quarteryear){
            return await _repository.YoutubeYear(quarteryear);
        }
        public async Task<List<StatisticYoutubeModel>> YoutubeYear_Singer(int quarteryear, string artistName){
            return await _repository.YoutubeYear_Singer(quarteryear, artistName);
        }
        public async Task<List<StatisticYoutubeModel>> YoutubeQuarter(int quarteryear, int quarter){
            return await _repository.YoutubeQuarter(quarteryear, quarter);
        }
        public async Task<List<StatisticYoutubeModel>> YoutubeQuarter_Singer(int quarteryear, int quarter, string artistName){
            return await _repository.YoutubeQuarter_Singer(quarteryear, quarter, artistName);
        }
        public async Task<List<StatisticPriceNameModel>> PriceYear(int quarteryear){
            return await _repository.PriceYear(quarteryear);
        }
        public async Task<List<StatisticPriceNameModel>> PriceYear_Singer(int quarteryear, string artistName){
            return await _repository.PriceYear_Singer(quarteryear, artistName);
        }
        public async Task<List<StatisticPriceNameModel>> PriceQuarter(int quarteryear, int quarter){
            return await _repository.PriceQuarter(quarteryear, quarter);
        }
        public async Task<List<StatisticPriceNameModel>> PriceQuarter_Singer(int quarteryear, int quarter, string artistName) {
            return await _repository.PriceQuarter_Singer(quarteryear, quarter, artistName);
        }
    }
}
