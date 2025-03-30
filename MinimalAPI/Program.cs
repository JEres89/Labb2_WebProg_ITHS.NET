//using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MinimalAPI.Endpoints;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using static System.Net.WebRequestMethods.Http;

namespace MinimalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "Webb Labb 2 MinimalAPI", 
                    Version = "v1",
					Description = "A minimal API using ASP.NET 8.0",
				});
			});
            builder.Services.AddSc

            var app = builder.Build();

            app.MapApiEndpoints();
			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }



            app.UseHttpsRedirection();



			app.Run();
        }
    }
}
