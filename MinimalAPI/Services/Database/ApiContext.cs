using Microsoft.EntityFrameworkCore;
using MinimalAPI.DataModels;

namespace MinimalAPI.Services.Database;

public class ApiContext : DbContext
{
	public ApiContext(DbContextOptions<ApiContext> options) : base(options)
	{

	}
	public DbSet<Customer> Customers { 
		get; 
		set; 
	}
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderProduct> OrderProducts { get; set; }
	public DbSet<Product> Products { get; set; }

	public Task<bool> CanConnectAsync()
	{
		return Database.CanConnectAsync();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Customer>().HasMany(c => c.Orders).WithOne(o => o.Customer).HasForeignKey(o => o.CustomerId);

		modelBuilder.Entity<Order>().HasMany(o => o.Products).WithOne(op => op.Order).HasForeignKey(op => op.OrderId);

		modelBuilder.Entity<Order>().Navigation(o => o.Products).AutoInclude();

		modelBuilder.Entity<OrderProduct>().HasKey(op => new { op.OrderId, op.ProductId });

		modelBuilder.Entity<OrderProduct>().Property(op => op.Price).HasColumnType("decimal(8,2)");

		modelBuilder.Entity<OrderProduct>().HasOne(op => op.Order).WithMany(o => o.Products).HasForeignKey(op => op.OrderId);

		modelBuilder.Entity<OrderProduct>().HasOne(op => op.Product).WithMany().HasForeignKey(op => op.ProductId);

		modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(8,2)");
	}
}
