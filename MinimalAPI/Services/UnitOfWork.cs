using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MinimalAPI.Services.Auth;
using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Database;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;
using System.Data.Common;
using static System.Net.HttpStatusCode;

namespace MinimalAPI.Services;

public class UnitOfWork : IUnitOfWork
{
	private bool disposedValue;
	private IDbContextTransaction? _transaction;

	public UnitOfWork(ApiContext context)
	{
		_context = context;
	}

	public ICustomersRepository Customers => _customers ??= new CustomersRepository(_context);
	public IOrdersRepository Orders => _orders ??= new OrdersRepository(_context); 
	public IProductsRepository Products => _products ??= new ProductsRepository(_context);
	public IAuthRepository Auth => _authRepository ??= new AuthRepository(_context);


	private readonly ApiContext _context;
	private CustomersRepository? _customers;
	private OrdersRepository? _orders;
	private ProductsRepository? _products;
	private AuthRepository? _authRepository;

	//public async Task<bool> BeginWork() => await _context.CanConnectAsync();

	public async Task<ValidationResult<T>> BeginWork<T>(bool writing)
	{
		if(!await _context.CanConnectAsync())
			return new ValidationResult<T> {
				ResultCode = ServiceUnavailable, 
				ErrorMessage = "Database connection failed." 
			};

		if(!writing || _transaction != null)
			return new ValidationResult<T> { 
				ResultCode = Continue 
			};

		try
		{
			_transaction = await _context.Database.BeginTransactionAsync();
			if(_transaction == null)
				throw new Exception("Transaction could not be initiated.");

			return new ValidationResult<T> { 
				ResultCode = Continue 
			};
		}
		catch(Exception e)
		{
			return new ValidationResult<T> { 
				ResultCode = InternalServerError, ErrorMessage = e.Message 
			};
		}
	}

	public async Task<ValidationResult<T>> SaveChangesAsync<T>()
	{
		if(_transaction == null)
			return new ValidationResult<T> { 
				ResultCode = Forbidden, 
				ErrorMessage = "Transaction has not been initiated." 
			};

		try
		{
			var changes = await _context.SaveChangesAsync();
			await _transaction.CommitAsync();
			Dispose();
			if(changes > 0)
				return new ValidationResult<T> {
					ResultCode = OK 
				};

			else
				return new ValidationResult<T> { 
					ResultCode =  BadRequest, 
					ErrorMessage = "No changes were saved." 
				};
		}
		catch(Exception e)
		{
			await RollbackAsync();
			if(e.InnerException is SqlException se)
				throw se;

			return new ValidationResult<T> { 
				ResultCode = InternalServerError, 
				ErrorMessage = $"Transaction could not be completed.\n{e.Message}" 
			};
		}
	}

	public async Task RollbackAsync()
	{
		if(_transaction != null)
		{
			await _transaction.RollbackAsync();
			_transaction.Dispose();
			_transaction = null;
			Dispose();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if(!disposedValue)
		{
			if(disposing)
			{
				_customers?.Dispose();
				_orders?.Dispose();
				_products?.Dispose();
				_authRepository?.Dispose();
				_transaction?.Dispose();
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
