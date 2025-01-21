using MusicManager.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.Services
{
    public class DataService : IDataService
    {
        private readonly IRepository<DataModel> _repository;
        private readonly IDataRepository _dataRepository;

        public DataService(IRepository<DataModel> repository, IDataRepository dataRepository)
        {
            _repository = repository;
            _dataRepository = dataRepository;
        }

        public void Create(DataModel input)
        {
            _repository.Add(input);
        }   
        public void AddRange(List<DataModel> input)
        {
            _repository.AddRange(input);
        }
        public async Task<List<DataModel>> GetDataExcel(int quarter, int year)
        {
           return await _dataRepository.GetData(quarter, year);
        }   
        public async Task<List<DataModel>> GetDataExcel(string artistName, int quarter, int year)
        {
            return await _dataRepository.GetData(artistName, quarter, year);
        }

    }
}
