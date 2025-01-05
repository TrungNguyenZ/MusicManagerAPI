using MusicManager.Models;
using MusicManager.Repositories;

namespace MusicManager.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _repository.GetAll();
        }

        public Product GetProductById(int id)
        {
            return _repository.GetById(id);
        }

        public void CreateProduct(Product product)
        {
            _repository.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _repository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            _repository.Delete(id);
        }
    }
}
