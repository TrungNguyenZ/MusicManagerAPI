using MusicManager.Models;

namespace MusicManager.Services
{
    public interface IRefreshTokenService
    {
        public void Create(RefreshToken input);
        public void AddRange(List<RefreshToken> input);
        public void Update(RefreshToken input);
        public IEnumerable<RefreshToken> GetAll();
    }
}
 