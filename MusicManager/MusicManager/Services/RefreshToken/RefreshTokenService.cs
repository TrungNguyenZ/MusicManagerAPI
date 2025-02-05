using MusicManager.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;

namespace MusicManager.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRepository<RefreshToken> _repository;

        public RefreshTokenService(IRepository<RefreshToken> repository)
        {
            _repository = repository;
        }

        public void Create(RefreshToken input)
        {
            _repository.Add(input);
        }   
        public void AddRange(List<RefreshToken> input)
        {
            _repository.AddRange(input);
        }
        public void Update(RefreshToken input)
        {
            _repository.Update(input);
        }
        public IEnumerable<RefreshToken> GetAll()
        {
            return _repository.GetAll();
        }

    }
}
