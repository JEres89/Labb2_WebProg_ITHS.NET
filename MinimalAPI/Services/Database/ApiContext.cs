using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;

namespace MinimalAPI.Services.Database;

public class ApiContext : DbContext
{
	public ApiContext(DbContextOptions<ApiContext> options) : base(options)
	{

	}
	public DbSet<WebUser> Users { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderProduct> OrderProducts { get; set; }
	public DbSet<Product> Products { get; set; }

	public void Init()
	{
		Console.WriteLine("Verifying database is initialized...");

		using(var sqlConnection = new SqlConnection(SecureSecretVault.ConnectionTestString))
			try
			{
				sqlConnection.Open();
			}
			catch(Exception)
			{
				if(false)
				{
					throw new Exception("Cannot connect to the database. Please check your connection string and database availability.");
				}
			}

		bool freshDb = false;
		if(Database.GetPendingMigrations().Any())
		{
			if(Database.GetAppliedMigrations().Any())
			{
				Console.WriteLine("Database schema is not up to date. Would you like to apply pending migrations? (y/n)");
				if(Console.ReadKey(true).Key != ConsoleKey.Y)
				{
					throw new Exception("Database migrations are pending. Please apply them before proceeding.");
				}
				Console.WriteLine("Applying pending migrations...");
			}
			else
			{
				freshDb = true;
				Console.WriteLine("Initializing database...");
			}
			try
			{
				Database.Migrate();
			}
			catch(Exception ex)
			{
				throw new Exception("Database migration failed. Please check your database configuration.", ex);
			}
		}

		if(freshDb)
		{
			Console.WriteLine("The database is empty, would you like to seed it with data? (y/n)");
			if(Console.ReadKey(true).Key == ConsoleKey.Y)
			{
				Console.WriteLine("Seeding database...");
				SeedDatabase();
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="timeout">Timeout in seconds.</param>
	/// <returns></returns>
	public Task<bool> CanConnectAsync(int timeout = 0)
	{
		if(timeout > 0)
		{
			try
			{
				return Database.CanConnectAsync(new CancellationTokenSource(timeout*1000).Token);
			}
			catch(OperationCanceledException)
			{
				return Task.FromResult(false);
			}
		}
		return Database.CanConnectAsync();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<WebUser>().HasKey(u => u.Email);
		modelBuilder.Entity<WebUser>().Property(u => u.Email).IsRequired().HasMaxLength(256);
		modelBuilder.Entity<WebUser>().Property(u => u.PasswordHash).IsRequired();
		modelBuilder.Entity<WebUser>().HasOne(u => u.Customer).WithOne().HasForeignKey<WebUser>(u => u.CustomerId)/*.HasForeignKey<Customer>(c => c.Email)*/.OnDelete(DeleteBehavior.SetNull);

		modelBuilder.Entity<Customer>().HasMany(c => c.Orders).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);

		modelBuilder.Entity<Order>().HasMany(o => o.Products).WithOne(op => op.Order).HasForeignKey(op => op.OrderId);

		modelBuilder.Entity<Order>().Navigation(o => o.Products).AutoInclude();

		modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId });

		modelBuilder.Entity<OrderProduct>().Property(op => op.Price).HasColumnType("decimal(8,2)");

		modelBuilder.Entity<OrderProduct>().HasOne(op => op.Order).WithMany(o => o.Products).HasForeignKey(op => op.OrderId);

		modelBuilder.Entity<OrderProduct>().HasOne(op => op.Product).WithMany().HasForeignKey(op => op.ProductId);

		modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(8,2)");
	}

	private void SeedDatabase()
	{
		var passwordHasher = new PasswordHasher<WebUser>();

		var products = new List<Product>
		{
			new Product { Name = "Laptop", Description = "High-performance laptop", Category = "Electronics", Price = 999.99m, Status = ProductStatus.Active, Stock = 50 },
			new Product { Name = "Smartphone", Description = "Latest model smartphone", Category = "Electronics", Price = 799.99m, Status = ProductStatus.Active, Stock = 100 },
			new Product { Name = "Headphones", Description = "Noise-cancelling headphones", Category = "Accessories", Price = 199.99m, Status = ProductStatus.Active, Stock = 200 },
			new Product { Name = "Desk Chair", Description = "Ergonomic desk chair", Category = "Furniture", Price = 149.99m, Status = ProductStatus.Active, Stock = 30 },
			new Product { Name = "Coffee Maker", Description = "Automatic coffee maker", Category = "Appliances", Price = 89.99m, Status = ProductStatus.Active, Stock = 75 },
			new Product { Name = "Gaming Console", Description = "Next-gen gaming console", Category = "Electronics", Price = 499.99m, Status = ProductStatus.Active, Stock = 40 },
			new Product { Name = "Electric Scooter", Description = "Eco-friendly electric scooter", Category = "Transportation", Price = 299.99m, Status = ProductStatus.Active, Stock = 25 },
			new Product { Name = "Smartwatch", Description = "Feature-packed smartwatch", Category = "Accessories", Price = 249.99m, Status = ProductStatus.Active, Stock = 150 },
			new Product { Name = "Backpack", Description = "Durable travel backpack", Category = "Accessories", Price = 59.99m, Status = ProductStatus.Active, Stock = 120 },
			new Product { Name = "LED Monitor", Description = "High-resolution LED monitor", Category = "Electronics", Price = 179.99m, Status = ProductStatus.Active, Stock = 80 }
		};

		Products.AddRange(products);

		var webUsers = new List<WebUser>
		{
			new WebUser { Email = "user1@example.com", PasswordHash = passwordHasher.HashPassword(null, "password1"), Role = Role.User },
			new WebUser { Email = "user2@example.com", PasswordHash = passwordHasher.HashPassword(null, "password2"), Role = Role.User },
			new WebUser { Email = "admin@example.com", PasswordHash = passwordHasher.HashPassword(null, "adminpassword"), Role = Role.Admin },
			new WebUser { Email = "guest@example.com", PasswordHash = passwordHasher.HashPassword(null, "guestpassword"), Role = Role.User }
		};

		Users.AddRange(webUsers);

		var customers = new List<Customer>
		{
			new()
			{
				FirstName = "John", LastName = "Doe", Email = "user1@example.com",
				Phone = "123-456-7890", Address = "123 Main St, Springfield",
				Orders = [
					new Order
					{
						CustomerId = 0, // EF Core will assign the generated ID
						Status = OrderStatus.New,
						Products = new List<OrderProduct>
						{
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[0], Count = 1, Price = products[0].Price },
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[1], Count = 2, Price = products[1].Price }
						}
					},
					new Order
					{
						CustomerId = 0, // EF Core will assign the generated ID
						Status = OrderStatus.Processing,
						Products = new List<OrderProduct>
						{
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[2], Count = 1, Price = products[2].Price },
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[3], Count = 1, Price = products[3].Price }
						}
					},
				]
			},
			new()
			{
				FirstName = "Jane", LastName = "Smith", Email = "user2@example.com",
				Phone = "987-654-3210", Address = "456 Elm St, Shelbyville",
				Orders = [
					new Order
					{
						CustomerId = 0, // EF Core will assign the generated ID
						Status = OrderStatus.Shipped,
						Products = new List<OrderProduct>
						{
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[4], Count = 1, Price = products[4].Price },
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[5], Count = 1, Price = products[5].Price }
						}
					},
					new Order
					{
						CustomerId = 0, // EF Core will assign the generated ID
						Status = OrderStatus.Delivered,
						Products = new List<OrderProduct>
						{
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[6], Count = 1, Price = products[6].Price },
							new OrderProduct { OrderId = 0, ProductId = 0, Product = products[7], Count = 1, Price = products[7].Price }
						}
					}
				]
			}
		};

		webUsers[0].Customer = customers[0];
		webUsers[1].Customer = customers[1];

		SaveChanges();
	}
}
