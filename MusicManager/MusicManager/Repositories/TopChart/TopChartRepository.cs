using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;

namespace MusicManager.Repositories
{
    public class TopChartRepository : ITopChartRepository
    {
        private readonly AppDbContext _context;

        public TopChartRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<TopChartArtist>> TopChartArt_Quarter(int quarter, int quarterYear)
        {
            var result = await _context.TopChartArtist
                .FromSqlRaw("EXEC SP_TopChart_Artist_Quarter @quarter = {0} , @quarterYear = {1}", quarter, quarterYear)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartArtist>> TopChartArt_Year(int year)
        {
            var result = await _context.TopChartArtist
                .FromSqlRaw("EXEC SP_TopChart_Artist_Year @year = {0}", year)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter(int quarter, int quarterYear)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Quarter  @quarter = {0}, @year = {1}", quarter, quarterYear)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year(int year)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Year  @year = {0}", year)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter_Singer(int quarter, int quarterYear, string artistName)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Quarter  @quarter = {0}, @year = {1}, @artistName", quarter, quarterYear, artistName)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year_Singer(int year, string artistName)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Year  @year = {0} , @artistName = {1}", year, artistName)
                .ToListAsync();
            return result;
        }
    }
}
