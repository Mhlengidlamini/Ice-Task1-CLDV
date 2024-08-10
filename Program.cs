using ABCRetail.AzureBlobService.Interface;
using ABCRetail.AzureBlobService.Service;
using ABCRetail.Repositories.RepositorieInterfaces;
using ABCRetail.Repositories.ServiceClasses;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Load Azure Storage connection string for the Tables
builder.Services.AddSingleton<TableServiceClient>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("AzureStorage");
    return new TableServiceClient(connectionString);
});
builder.Services.AddSingleton<IBlobStorageService>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("BlobStorage");
    return new BlobStorageService(connectionString);
});
// Register the repository and service for the Azure Tables
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
