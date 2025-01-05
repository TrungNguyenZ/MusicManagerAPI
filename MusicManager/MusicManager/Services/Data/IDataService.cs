using MusicManager.Models;

namespace MusicManager.Services
{
    public interface IDataService
    {
        void Create(DataModel input);
        void AddRange(List<DataModel> input);
    }
}
 