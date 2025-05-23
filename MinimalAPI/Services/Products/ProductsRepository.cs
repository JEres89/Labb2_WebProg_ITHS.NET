using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MinimalAPI.DataModels;
using MinimalAPI.Services.Database;

namespace MinimalAPI.Services.Products;

public class ProductsRepository : IProductsRepository
{
	private readonly ApiContext _context;
	public ProductsRepository(ApiContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<Product>> GetProductsAsync()
	{
		return await _context.Products.ToListAsync();
	}

	public async Task<Product> CreateProductAsync(Product product)
	{
		return (await _context.Products.AddAsync(product)).Entity;
		//return product;
	}

	public async Task<Product?> GetProductAsync(int id)
	{
		return await _context.Products.FindAsync(id);
	}

	public async Task<Product?> UpdateProductAsync(int id, Dictionary<string, string> updates)
	{
		var product = await _context.Products.FindAsync(id);
		if(product == null)
		{
			return null;
		}
		var entity = _context.Entry(product);

		foreach(var prop in updates)
		{
			entity.SetProperty(prop.Key, prop.Value);
		}

		return product;
	}

	public async Task<bool> DeleteProductAsync(int id)
	{
		var product = await GetProductAsync(id);
		if(product == null)
		{
			return false;
		}

		_context.Products.Remove(product);
		//await _context.SaveChangesAsync();

		return true;
	}

	//public async Task<IEnumerable<Product>> FindProductsAsync(Predicate<Product> productMatch)
	//{
	//	return await _context.Products.Where(c => productMatch(c)).ToListAsync();
	//	//return await Task.FromResult(_context.Products.AsEnumerable().Where(c => productMatch(c)).ToList());
	//}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}
