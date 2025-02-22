using MusicManager.Models;
using MusicManager.Repositories;

namespace MusicManager.Services.TopChart
{
    public class TopChartService : ITopChartService
    {
        private readonly ITopChartRepository _repository;
        private readonly ICommonService _commonService;

        public TopChartService(ITopChartRepository repository, ICommonService commonService)
        {
            _repository = repository;
            _commonService = commonService;
        }
        public async Task<List<TopChartArtist>> TopChartArt_Quarter(int quarter, int quarterYear, int pageSize = 5)
        {
            return await _repository.TopChartArt_Quarter(quarter, quarterYear, pageSize);
        }
        public async Task<List<TopChartArtist>> TopChartArt_Year(int year, int pageSize = 5)
        {
            return await _repository.TopChartArt_Year(year, pageSize);
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter(int quarter, int quarterYear, int pageSize = 5) { 
            var data = await _repository.TopChartTrack_Quarter(quarter, quarterYear, pageSize);
            var rs = data.Select(x=> new TopChartTrack { catalogueTitle = x.catalogueTitle, artistName = x.artistName, sum = _commonService.GetNetEnterprise(x.sum),percentage = x.percentage }).ToList();
            return rs;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year(int year, int pageSize = 5) {
            return await _repository.TopChartTrack_Year(year, pageSize);
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter_Singer(int quarter, int quarterYear, string artistName, int pageSize = 5)
        {
            return await _repository.TopChartTrack_Quarter_Singer(quarter, quarterYear, artistName, pageSize);
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year_Singer(int year, string artistName, int pageSize = 5)
        {
            return await _repository.TopChartTrack_Year_Singer(year, artistName, pageSize);
        }

    }
}
