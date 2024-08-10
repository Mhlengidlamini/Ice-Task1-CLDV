using ABCRetail.Models;
using ABCRetail.Repositories.RepositorieInterfaces;
using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Repositories.ServiceClasses
{
    public class ProductService : IProductService
    {
        private readonly TableClient _tableClient;

        public ProductService(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("ProductTable");
            _tableClient.CreateIfNotExists();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in _tableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<Product>("Product", id);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
        }

        public async Task AddProductAsync(Product product)
        {
            try
            {
                await _tableClient.AddEntityAsync(product);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            try
            {
                await _tableClient.UpdateEntityAsync(product, ETag.All, TableUpdateMode.Replace);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteProductAsync(string id)
        {
            try
            {
                await _tableClient.DeleteEntityAsync("Product", id);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetNextProductRowKeyAsync()
        {
            var products = await GetAllProductsAsync();
            var lastProduct = products.OrderByDescending(p => int.Parse(p.RowKey.Substring(1))).FirstOrDefault();
            if (lastProduct == null)
            {
                return "P0";
            }

            int nextId = int.Parse(lastProduct.RowKey.Substring(1)) + 1;
            return $"P{nextId}";
        }
    }
}
