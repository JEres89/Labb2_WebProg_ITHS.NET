using BlazorFrontend.Components;
using BlazorFrontend.Services;

namespace BlazorFrontend;
public class Program
{
	private const string API_URL = "https://localhost:7255";
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services
			.AddRazorComponents()
			.AddInteractiveServerComponents();

		builder.Services
			.AddScoped<ProductService>()
			.AddScoped<AuthService>()
			.AddScoped<CustomerService>()
			.AddScoped<OrderService>();

		builder.Services.AddHttpClient("APIclient", client =>
			{
				client.BaseAddress = new Uri(API_URL);
				client.Timeout = TimeSpan.FromSeconds(120);
			});
			//.AddTypedClient<ProductService>()
			//.AddTypedClient<AuthService>()
			//.AddTypedClient<CustomerService>();

		builder.Services.AddScoped(sp =>
		{
			var factory = sp.GetRequiredService<IHttpClientFactory>();
			return factory.CreateClient("APIclient");
		});

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if(!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();
		app.UseAntiforgery();

		app.MapRazorComponents<App>()
			.AddInteractiveServerRenderMode();

		app.Run();
	}
}
