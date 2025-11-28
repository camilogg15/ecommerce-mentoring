using Asp.Versioning;
using CartService.Application.Contracts.Messaging;
using CartService.Application.Events;
using CartService.Application.Handlers;
using CartService.Application.Services.Cart;
using CartService.Application.Services.Dispatcher;
using CartService.Application.Validators;
using CartService.Domain.Interfaces;
using CartService.Domain.Models;
using CartService.Infrastructure.LiteDb;
using CartService.Infrastructure.Messaging;
using FluentValidation;
using Microsoft.OpenApi.Models;

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

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            // Add settings
            builder.Services.Configure<LiteDbSettings>(builder.Configuration.GetSection("LiteDbSettings"));

            // Add validators
            builder.Services.AddScoped<IValidator<CartItem>, CartItemValidator>();

            // Add services
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, Application.Services.Cart.CartService>();

            builder.Services.AddSingleton<MessageDispatcher>();
            builder.Services.AddSingleton<IMessageListener, RabbitMqListener>();

            // Handlers
            builder.Services.AddTransient<IMessageHandler, ProductUpdatedHandler>();
            builder.Services.AddSingleton<MessageDispatcher>();

            // Hosted service
            builder.Services.AddHostedService<MessageListenerHostedService>();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Cart Service API",
                    Version = "v1"
                });
                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Cart Service API",
                    Version = "v2"
                });
                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API v1");
                    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Cart API v2");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
