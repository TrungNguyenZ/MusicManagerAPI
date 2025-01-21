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
        public async Task<List<TopChartArtist>> TopChartArt_Quarter(int quarter, int quarterYear){
            return await _repository.TopChartArt_Quarter(quarter, quarterYear);
        }
        public async Task<List<TopChartArtist>> TopChartArt_Year(int year){
            return await _repository.TopChartArt_Year(year);
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter(int quarter, int quarterYear) { 
            return await _repository.TopChartTrack_Quarter(quarter, quarterYear);
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year(int year) {
            return await _repository.TopChartTrack_Year(year);
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter_Singer(int quarter, int quarterYear, string artistName)
        {
            return await _repository.TopChartTrack_Quarter_Singer(quarter, quarterYear, artistName);
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year_Singer(int year, string artistName)
        {
            return await _repository.TopChartTrack_Year_Singer(year, artistName);
        }

    }
}
