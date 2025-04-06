using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Database;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;

namespace MinimalAPI.Services;

public class UnitOfWork : IUnitOfWork
{
	private bool disposedValue;

	public UnitOfWork(ICustomersRepository customers, IOrdersRepository orders, IProductsRepository products, ApiContext context)
	{
		Customers = customers;
		Orders = orders;
		Products = products;
		_context = context;
	}

	public ICustomersRepository Customers { get; }
	public IOrdersRepository Orders { get; }
	public IProductsRepository Products { get; }

	private readonly ApiContext _context;


	public async Task<int> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync();
	}

	protected virtual void Dispose(bool disposing)
	{
		if(!disposedValue)
		{
			if(disposing)
			{
				// TODO: dispose managed state (managed objects)
				Customers.Dispose();
				Orders.Dispose();
				Products.Dispose();
			}

			// TODO: free unmanaged resources (unmanaged objects) and override finalizer
			// TODO: set large fields to null
			disposedValue=true;
		}
	}

	// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
	// ~UnitOfWork()
	// {
	//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
	//     Dispose(disposing: false);
	// }

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
