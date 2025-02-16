using MusicManager.Models;
using MusicManager.Repositories;

namespace MusicManager.Services.TopChart
{
    public class TopChartService : ITopChartService
    {
        private readonly ITopChartRepository _repository;

        public TopChartService(ITopChartRepository repository)
        {
            _repository = repository;
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
            return await _repository.TopChartTrack_Quarter(quarter, quarterYear, pageSize);
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
