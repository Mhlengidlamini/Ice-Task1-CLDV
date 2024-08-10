using ABCRetail.Models;
using ABCRetail.Repositories.RepositorieInterfaces;
using ABCRetail.ViewModel;
using Azure;
using Azure.Data.Tables;

namespace ABCRetail.Repositories.ServiceClasses
{
    public class CustomerService : ICustomerService
    {
        private readonly TableClient _tableClient;

        public CustomerService(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient("CustomerProfiles");
            _tableClient.CreateIfNotExists();
        }

        public async Task<List<CustomerProfile>> GetAllCustomersAsync()
        {
            var customers = new List<CustomerProfile>();
            await foreach (var customer in _tableClient.QueryAsync<CustomerProfile>())
            {
                customers.Add(customer);
            }
            return customers;
        }


        public async Task<CustomerProfile> GetCustomerByIdAsync(string id)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<CustomerProfile>("Customer", id);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Return null if the customer does not exist
                return null;
            }
        }

        public async Task AddCustomerAsync(CustomerProfile customer)
        {
            try
            {
                await _tableClient.AddEntityAsync(customer);
            }
            catch (RequestFailedException ex)
            {
                // Handle the error
                Console.WriteLine($"Error adding or updating entity: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateCustomerAsync(CustomerProfile customer)
        {
            try
            {
                await _tableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
            }
            catch (RequestFailedException ex)
            {
                // Handle the error as needed
                Console.WriteLine($"Error editing entity: {ex.Message}");
                throw;
            }
        }
        public async Task DeleteCustomerAsync(string id)
        {
            try 
            {
                await _tableClient.DeleteEntityAsync("Customer", id);
            }
            catch (RequestFailedException ex)
            {
                // Handle the error as needed
                Console.WriteLine($"Error deleting entity: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetNextCustomerRowKeyAsync()
        {
            var customers = await GetAllCustomersAsync();
            var lastCustomer = customers.OrderByDescending(c => int.Parse(c.RowKey.Substring(1))).FirstOrDefault();
            if (lastCustomer == null)
            {
                return "C0";
            }

            int nextId = int.Parse(lastCustomer.RowKey.Substring(1)) + 1;
            return $"C{nextId}";
        }
    }
}
