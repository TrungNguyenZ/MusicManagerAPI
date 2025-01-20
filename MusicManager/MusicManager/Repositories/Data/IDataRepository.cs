using MusicManager.Models;

namespace MusicManager.Repositories.Data
{
    public interface IDataRepository
    {
        Task<List<DataModel>> GetData(int quarter, int year);
        Task<List<DataModel>> GetData(string artistName, int quarter, int year);
    }
}
