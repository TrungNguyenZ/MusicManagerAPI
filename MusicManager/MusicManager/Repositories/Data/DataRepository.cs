using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;

namespace MusicManager.Repositories.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly AppDbContext _context;

        public DataRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<DataExportExcelModel>> GetData(int quarter, int year)
        {
            var result = await _context.DataExportExcelModel
                .FromSqlRaw("EXEC SP_Data_Quarter @quarter = {0} , @year = {1}", quarter, year)
                .ToListAsync();
            return result;
        }
        public async Task<List<DataExportExcelModel>> GetData(string artistName, int quarter, int year)
        {
            var result = await _context.DataExportExcelModel
                .FromSqlRaw("EXEC SP_Data_Quarter_Singer @artistName = {0} , @quarter = {1} , @year = {2}", artistName, quarter, year)
                .ToListAsync();
            return result;
        }

    }
}
