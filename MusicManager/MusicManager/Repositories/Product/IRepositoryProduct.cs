using MusicManager.Models;

namespace MusicManager.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task AddProductAsync(string name, decimal price);
        Task<int> AddProductWithOutputAsync(string name, decimal price);
    }
}
