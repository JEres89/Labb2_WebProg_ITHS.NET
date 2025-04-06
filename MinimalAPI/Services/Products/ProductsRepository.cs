using Microsoft.EntityFrameworkCore;
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
		_context.Products.Add(product);
		return product;
	}

	public async Task<Product?> GetProductAsync(int id)
	{
		return await _context.Products.FindAsync(id);
	}

	public async Task<Product?> UpdateProductAsync(int id, Dictionary<string, string> updates)
	{
		//Product product = new() { Id = id, FirstName = "", LastName = "", Email = "" };
		//var entity = _context.Attach<Product>(product);

		var product = await _context.Products.FindAsync(id);
		if(product == null)
		{
			return null;
		}
		var entity = _context.Entry(product);

		//TODO: Add checking for case-incorrect property names
		foreach(var prop in updates)
		{
			entity.Member(prop.Key).CurrentValue = prop.Value;
		}

		return product;
		//var changes = await _context.SaveChangesAsync(true);

		//return changes > 0 ? product : null;
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
