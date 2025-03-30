namespace MinimalAPI.Services;

public interface IUnitOfWork : IDisposable
{
	ICustomersRepository Customers { get; }
	IOrdersRepository Orders { get; }
	//IProductsRepository Products { get; }
	Task<int> SaveChangesAsync();
}
