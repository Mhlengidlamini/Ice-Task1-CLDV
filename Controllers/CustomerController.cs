using Microsoft.AspNetCore.Mvc;
using ABCRetail.Models;
using ABCRetail.ViewModel;
using ABCRetail.Repositories.ServiceClasses;
using ABCRetail.Repositories.RepositorieInterfaces;
using Azure;

public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET: /Customer
    public async Task<IActionResult> Index()
    {
        try
        {
            var customers = await _customerService.GetAllCustomersAsync();
            var viewModel = customers.Select(c => new CustomerViewModel
            {
                Id = c.RowKey, // Directly use RowKey as string
                Name = c.Name,
                Email = c.Email,
                Address = c.Address,
                PhoneNumber = c.PhoneNumber
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving customers: {ex.Message}");
            return View(new List<CustomerViewModel>());
        }
    }



    // GET: /Customer/Details
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerViewModel
            {
                // Parse RowKey safely; assuming RowKey is an integer
                Id = customer.RowKey,
                Name = customer.Name,
                Email = customer.Email,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving customer details: {ex.Message}");
            return View();
        }
    }


    //GET
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Customer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var rowKey = await _customerService.GetNextCustomerRowKeyAsync();
                var customer = new CustomerProfile
                {
                    PartitionKey = "Customer", // Set appropriate PartitionKey
                    RowKey = rowKey, // Use generated RowKey
                    Name = model.Name,
                    Email = model.Email,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
                await _customerService.AddCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the customer: {ex.Message}");
            }
        }
        return View(model);
    }

    // GET: /Customer/Edit
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound(); // Customer not found
            }

            var viewModel = new CustomerViewModel
            {
                Id = customer.RowKey, // Directly use RowKey as string
                Name = customer.Name,
                Email = customer.Email,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving customer details: {ex.Message}");
            return View();
        }
    }


    // POST: /Customer/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CustomerViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var customer = new CustomerProfile
                {
                    PartitionKey = "Customer", // Set appropriate PartitionKey
                    RowKey = model.Id.ToString(), // Use Id as RowKey
                    Name = model.Name,
                    Email = model.Email,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
                await _customerService.UpdateCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                ModelState.AddModelError("", "The customer could not be found.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while updating the customer: {ex.Message}");
            }
        }
        return View(model);
    }

    // GET: /Customer/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerViewModel
            {
                Id = customer.RowKey, // Assuming RowKey is an integer
                Name = customer.Name,
                Email = customer.Email,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving customer details: {ex.Message}");
            return View();
        }
    }

    // POST: /Customer/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            await _customerService.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while deleting the customer: {ex.Message}");
            return View();
        }
    }
}

