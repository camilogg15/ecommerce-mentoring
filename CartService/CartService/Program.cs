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
using CartService.Infrastructure.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace CartService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", opts =>
            {
                opts.Authority = "http://localhost:8080/realms/store-auth";
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
                opts.RequireHttpsMetadata = false;

                opts.Events = new JwtBearerEvents
                {
                    OnTokenValidated = ctx =>
                    {
                        var identity = ctx.Principal!.Identity as ClaimsIdentity;
                        var realmAccess = ctx.Principal.FindFirst("realm_access")?.Value;

                        if (realmAccess != null)
                        {
                            var data = System.Text.Json.JsonDocument.Parse(realmAccess);
                            if (data.RootElement.TryGetProperty("roles", out var rolesElement))
                            {
                                foreach (var r in rolesElement.EnumerateArray())
                                {
                                    identity!.AddClaim(new Claim(ClaimTypes.Role, r.GetString()!));
                                }
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ManagerOnly", policy =>
                    policy.RequireRole("manager"));

                options.AddPolicy("CustomerOrManager", policy =>
                    policy.RequireRole("customer", "manager"));
            });

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
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter the JWT in the format: Bearer {token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

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

            app.UseMiddleware<TokenLoggingMiddleware>();

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
