using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPI.Auth;
using MinimalAPI.Endpoints;
using MinimalAPI.Services;
using MinimalAPI.Services.Auth;
using MinimalAPI.Services.Customers;
using MinimalAPI.Services.Database;
using MinimalAPI.Services.Orders;
using MinimalAPI.Services.Products;
using System.Reflection.Emit;

namespace MinimalAPI;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services
			.AddAuthentication(s =>
			{
				s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(j =>
			{
				j.TokenValidationParameters = new TokenValidationParameters {
					ValidIssuer = "JensEresund",
					ValidAudience = "ApiConsumers",
					IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecureSecretVault.GetJwtSecret())),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
				};
			});

		builder.Services
			.AddAuthorizationBuilder()
			.AddPolicy("Administrator", policy =>
			{
				policy.RequireRole(Role.Admin.ToString());
			});
			//.AddPolicy("OwnerOrAdmin", policy =>
			//{
			//	policy.RequireRole(Role.User.ToString(), Role.Admin.ToString());
			//	policy.RequireAssertion(context =>
			//	{
			//		var user = context.User;
			//		if(user.IsInRole(Role.Admin.ToString()))
			//			return true;
			//		if(user.IsInRole(Role.User.ToString()) && user.HasClaim(c => c.Type == "CustomerId"))
			//		{
			//			var customerId = int.Parse(user.FindFirst("CustomerId")!.Value);
			//			return context.Resource is int hasCustomerId && hasCustomerId == customerId;
			//		}
			//		return false;
			//	});
			//});
		#if DEBUG
		builder.Services
			.AddScoped<WebUser>(_ => new WebUser { Email = "Asd", Role = Role.Admin });
		#endif

		builder.Services
			.AddScoped<IUnitOfWork, UnitOfWork>()
			.AddTransient<ICustomersActionValidationService, CustomersActionValidationService>()
			.AddTransient<IProductsActionValidationService, ProductsActionValidationService>()
			.AddTransient<IOrdersActionValidationService, OrdersActionValidationService>()
			.AddTransient<IAuthActionValidationService, AuthActionValidationService>();

		builder.Services.AddDbContext<ApiContext>(options => options.UseSqlServer(SecureSecretVault.GetConnectionString()));

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

		// Initialization logic here
		EnsureAdminUserExists(app);

		// Configure the HTTP request pipeline.
		if(app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapApiEndpoints();
		app.Run();
	}

	private static void EnsureAdminUserExists(WebApplication app)
	{
		var configuration = app.Configuration;
		var adminEmail = configuration.GetValue<string>("adminEmail");
		var adminPassword = configuration.GetValue<string>("adminPassword");
		
		using(var scope = app.Services.CreateScope())
		using(var context = scope.ServiceProvider.GetRequiredService<ApiContext>())
		{
			context.Init();

			if(adminEmail is not null && adminPassword is not null)
			{
				var adminUser = new WebUser {
					Email = adminEmail,

					Role = Role.Admin
				};
				adminUser.PasswordHash = new PasswordHasher<WebUser>().HashPassword(adminUser, adminPassword!);

				context.Users.Add(adminUser);
				var saveTask = context.SaveChangesAsync();
				saveTask.RunSynchronously();

				// Clear the admin credentials from the configuration to prevent reuse and potential security issues
				configuration["adminPassword"] = null;
				configuration["adminEmail"] = null;

				if(saveTask.Result <= 0)
				{
					throw new Exception("Failed to create admin user in the database.");
				}
			}

			var users = context.Users.Where(u => u.Role == Role.Admin).ToList();
			if(users.Count == 0)
			{
				#if DEBUG
				// In debug mode, we can create a default admin user with debug credentials
				var user = new WebUser {
					Email = SecureSecretVault.DebugUserEmail,
					Role = Role.Admin
				};
				user.PasswordHash = new PasswordHasher<WebUser>().HashPassword(user, SecureSecretVault.DebugUserPassword);
				context.Users.Add(user);
				context.SaveChanges();
				return;
				#endif

				throw new Exception("There is no admin user in the database and no admin credentials were provided. Server cannot start without an administrator.");
			}
		}
	}
}