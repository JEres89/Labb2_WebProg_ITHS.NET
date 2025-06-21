using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace MinimalAPI.Services.Products;

public interface IProductsRepository : IDisposable
{
	Task<IEnumerable<Product>> GetProductsAsync();
	Task<Product> CreateProductAsync(Product product);
	Task<Product?> GetProductAsync(int id);
	Task<Product?> UpdateProductAsync(int id, Dictionary<string, string> updates);
	Task<bool> DeleteProductAsync(int id);
}

public interface IProductsActionValidationService
{
	Task<ValidationResult<IEnumerable<Product>>> GetProductsAsync();
	Task<ValidationResult<Product>> CreateProductAsync(Product product);
	Task<ValidationResult<Product>> GetProductAsync(int id);
	Task<ValidationResult<Product>> UpdateProductAsync(int id, Dictionary<string, string> product);
	Task<ValidationResult<int>> DeleteProductAsync(int id);
	Task<ValidationResult<IEnumerable<OrderProduct>>> GetProductOrdersAsync(int id);
}
