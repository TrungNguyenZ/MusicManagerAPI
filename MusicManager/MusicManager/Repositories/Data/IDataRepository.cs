using MusicManager.Models;

namespace MusicManager.Repositories.Data
{
    public interface IDataRepository
    {
        Task<List<DataExportExcelModel>> GetData(int quarter, int year);
        Task<List<DataExportExcelModel>> GetData(string artistName, int quarter, int year);
    }
}
