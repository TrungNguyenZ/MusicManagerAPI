using MusicManager.Models;

namespace MusicManager.Services
{
    public interface IDataService
    {
        void Create(DataModel input);
        void AddRange(List<DataModel> input);
        Task<List<DataModel>> GetDataExcel(int quarter, int year);
        Task<List<DataModel>> GetDataExcel(string artistName, int quarter, int year);
    }
}
 