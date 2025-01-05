using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;

namespace MusicManager.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // Gọi Stored Procedure để lấy tất cả sản phẩm
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .FromSqlRaw("EXEC GetAllProducts")
                .ToListAsync();
        }

        // Gọi Stored Procedure để thêm sản phẩm mới
        public async Task AddProductAsync(string name, decimal price)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC AddProduct @Name, @Price",
                new[]
                {
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Price", price)
                });
        }

        // Gọi Stored Procedure trả về giá trị OUTPUT
        public async Task<int> AddProductWithOutputAsync(string name, decimal price)
        {
            var outputId = new SqlParameter("@NewProductId", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC AddProductWithOutput @Name, @Price, @NewProductId OUTPUT",
                new[]
                {
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Price", price),
                    outputId
                });

            return (int)outputId.Value;
        }
    }
}
