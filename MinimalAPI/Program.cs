using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalAPI.Endpoints;
using MinimalAPI.Services;
using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Database;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;

namespace MinimalAPI;

public class Program
{
	private const string ConnectionString =
		"Server=localhost;" +
		"Database=WebbLabb2;" +
		"Trusted_Connection=True;" +
		"TrustServerCertificate=True";
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddAuthorization();

		builder.Services
			.AddScoped<IUnitOfWork, UnitOfWork>()
			.AddScoped<ICustomersRepository, CustomersRepository>()
			.AddScoped<IOrdersRepository, OrdersRepository>()
			.AddScoped<IProductsRepository, ProductsRepository>();

		builder.Services
			.AddTransient<ICustomersActionValidationService, CustomersActionValidationService>()
			.AddTransient<IProductsActionValidationService, ProductsActionValidationService>()
			.AddTransient<IOrdersActionValidationService, OrdersActionValidationService>();

		builder.Services.AddDbContext<ApiContext>(options => options.UseSqlServer(ConnectionString));

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Webb Labb 2 MinimalAPI",
				Version = "v1",
				Description = "A minimal API using ASP.NET 8.0",
			});
		});

		var app = builder.Build();

		app.MapApiEndpoints();
		// Configure the HTTP request pipeline.
		if(app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}



		app.UseHttpsRedirection();



		app.Run();
	}
}