using ABCRetail.Models;

namespace ABCRetail.Repositories.RepositorieInterfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(string id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(string id);
        Task<string> GetNextProductRowKeyAsync();
    }
}
