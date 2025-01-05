using MusicManager.Models;
using MusicManager.Repositories;

namespace MusicManager.Services
{
    public class DataService : IDataService
    {
        private readonly IRepository<DataModel> _repository;

        public DataService(IRepository<DataModel> repository)
        {
            _repository = repository;
        }

        public void Create(DataModel input)
        {
            _repository.Add(input);
        }   
        public void AddRange(List<DataModel> input)
        {
            _repository.AddRange(input);
        }

    }
}
