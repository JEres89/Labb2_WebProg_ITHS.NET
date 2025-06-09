using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
}