using MinimalAPI.Services.Auth;
using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;

namespace MinimalAPI.Services;

public interface IUnitOfWork : IDisposable
{
	ICustomersRepository Customers { get; }
	IOrdersRepository Orders { get; }
	IProductsRepository Products { get; }
	IAuthRepository Auth { get; }
	//Task<bool> BeginWork();
	Task<ValidationResult<T>> BeginWork<T>(bool writing);
	Task<ValidationResult<T>> SaveChangesAsync<T>();
	Task RollbackAsync();
}
