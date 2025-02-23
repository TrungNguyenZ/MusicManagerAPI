using MusicManager.Models;

namespace MusicManager.Services
{
    public interface IDataService
    {
        void Create(DataModel input);
        void AddRange(List<DataModel> input);
        Task<List<DataExportExcelModel>> GetDataExcel(int quarter, int year);
        Task<List<DataExportExcelModel>> GetDataExcel(string artistName, int quarter, int year);
        Task<List<TableRevenue>> GetTableRevenue(int quarter, int year);
    }
}
 