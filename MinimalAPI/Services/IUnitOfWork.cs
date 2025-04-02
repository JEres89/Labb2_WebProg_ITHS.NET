using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;

namespace MinimalAPI.Services;

public interface IUnitOfWork : IDisposable
{
	ICustomersRepository Customers { get; }
	IOrdersRepository Orders { get; }
	IProductsRepository Products { get; }
	Task<int> SaveChangesAsync();
}
