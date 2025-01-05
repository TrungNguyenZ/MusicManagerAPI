using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;

namespace MusicManager.Repositories.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly AppDbContext _context;

        public DataRepository(AppDbContext context)
        {
            _context = context;
        }


    }
}
