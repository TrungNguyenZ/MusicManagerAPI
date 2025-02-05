using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;

namespace MusicManager.Repositories.Data
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
    }
}
