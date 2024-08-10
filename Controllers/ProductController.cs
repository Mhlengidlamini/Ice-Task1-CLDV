using Microsoft.AspNetCore.Mvc;
using ABCRetail.Models;
using ABCRetail.ViewModel;
using ABCRetail.Repositories.ServiceClasses;
using ABCRetail.Repositories.RepositorieInterfaces;
using Azure;
using ABCRetail.AzureBlobService.Service;
using ABCRetail.AzureBlobService.Interface;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly IBlobStorageService _blobStorageService;

    public ProductController(IProductService productService, IBlobStorageService blobStorageService)
    {
        _productService = productService;
        _blobStorageService = blobStorageService;
    }

    // GET: /Product
    public async Task<IActionResult> Index()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            var viewModel = products.Select(p => new ProductViewModel
            {
                Id = p.RowKey, // Directly use RowKey as string
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                StockLevel = p.StockLevel
            }).ToList();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving products: {ex.Message}");
            return View(new List<ProductViewModel>());
        }
    }

    // GET: /Product/Details
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.RowKey,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockLevel = product.StockLevel
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving product details: {ex.Message}");
            return View();
        }
    }

    // GET: /Product/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var rowKey = await _productService.GetNextProductRowKeyAsync();
                var product = new Product
                {
                    PartitionKey = "Product",
                    RowKey = model.Id,
                    ProductName = model.ProductName,
                    Description = model.Description,
                    Price = model.Price,
                    StockLevel = model.StockLevel
                };
                await _productService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while creating the product: {ex.Message}");
            }
        }
        return View(model);
    }

    // GET: /Product/Edit
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.RowKey,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockLevel = product.StockLevel
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving product details: {ex.Message}");
            return View();
        }
    }

    // POST: /Product/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var product = new Product
                {
                    PartitionKey = "Product",
                    RowKey = model.Id, // Use Id as RowKey
                    ProductName = model.ProductName,
                    Description = model.Description,
                    Price = model.Price,
                    StockLevel = model.StockLevel
                };
                await _productService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                ModelState.AddModelError("", "The product could not be found.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while updating the product: {ex.Message}");
            }
        }
        return View(model);
    }

    // GET: /Product/Delete
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.RowKey,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                StockLevel = product.StockLevel
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while retrieving product details: {ex.Message}");
            return View();
        }
    }

    // POST: /Product/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred while deleting the product: {ex.Message}");
            return View();
        }
    }

    [HttpGet]
    public IActionResult UploadProductImage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadProductImage(IFormFile file)
    {
        try
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var fileName = file.FileName;
                    var blobUrl = await _blobStorageService.UploadFileAsync(fileName, stream);
                    ViewData["BlobUrl"] = blobUrl;
                }
            }
            else
            {
                // Handle the case where no file was uploaded
                ViewData["Error"] = "No file selected for upload.";
            }
        }
        catch (Exception ex)
        {
            // Log the exception and show an error message to the user
            Console.WriteLine($"Error in UploadProductImage: {ex.Message}");
            ViewData["Error"] = "An error occurred while uploading the file. Please try again.";
        }

        return View();
    }

}
