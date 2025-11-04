using CartService.Application.Services.Cart;
using CartService.Application.Validators;
using CartService.Domain.Interfaces;
using CartService.Domain.Models;
using CartService.Infrastructure.LiteDb;
using FluentValidation;

namespace CartService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Add settings
            builder.Services.Configure<LiteDbSettings>(builder.Configuration.GetSection("LiteDbSettings"));

            // Add validators
            builder.Services.AddScoped<IValidator<CartItem>, CartItemValidator>();

            // Add services
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, Application.Services.Cart.CartService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
