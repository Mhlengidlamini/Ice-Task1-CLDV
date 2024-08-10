using ABCRetail.Models;
using ABCRetail.ViewModel;

namespace ABCRetail.Repositories.RepositorieInterfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerProfile>> GetAllCustomersAsync();
        Task<CustomerProfile> GetCustomerByIdAsync(string id);
        Task AddCustomerAsync(CustomerProfile customer);
        Task UpdateCustomerAsync(CustomerProfile customer);
        Task DeleteCustomerAsync(string id);
        Task<string> GetNextCustomerRowKeyAsync();
    }
}
