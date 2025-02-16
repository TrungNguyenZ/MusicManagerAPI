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
        public async Task<List<TopChartArtist>> TopChartArt_Quarter(int quarter, int quarterYear, int pageSize = 5)
        {
            var result = await _context.TopChartArtist
                .FromSqlRaw("EXEC SP_TopChart_Artist_Quarter @quarter = {0} , @quarterYear = {1}, @pageSize = {2} ", quarter, quarterYear, pageSize)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartArtist>> TopChartArt_Year(int year, int pageSize = 5)
        {
            var result = await _context.TopChartArtist
                .FromSqlRaw("EXEC SP_TopChart_Artist_Year @year = {0}, @pageSize = {1} ", year, pageSize)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter(int quarter, int quarterYear, int pageSize = 5)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Quarter  @quarter = {0}, @year = {1}, @pageSize = {2}", quarter, quarterYear, pageSize)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year(int year, int pageSize = 5)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Year  @year = {0}, @pageSize = {1}", year, pageSize)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Quarter_Singer(int quarter, int quarterYear, string artistName, int pageSize = 5)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Quarter_Singer  @quarter = {0}, @year = {1}, @artistName = {2}, @pageSize = {3}", quarter, quarterYear, artistName, pageSize)
                .ToListAsync();
            return result;
        }
        public async Task<List<TopChartTrack>> TopChartTrack_Year_Singer(int year, string artistName, int pageSize = 5)
        {
            var result = await _context.TopChartTrack
                .FromSqlRaw("EXEC SP_TopChart_Track_Year_Singer  @year = {0} , @artistName = {1},  @pageSize = {2}", year, artistName, pageSize)
                .ToListAsync();
            return result;
        }
    }
}
