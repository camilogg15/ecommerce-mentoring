using System.Security.Claims;

using Asp.Versioning;

using CartService.Application.Contracts.Messaging;
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

using Swashbuckle.AspNetCore.SwaggerGen;

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
                opts.Authority = "http://host.docker.internal:8080/realms/store-auth";
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
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
            builder.Services.AddHostedService<RabbitMqListener>();

            // Handlers
            builder.Services.AddTransient<IMessageHandler, ProductUpdatedHandler>();

            builder.Services.AddTransient<Microsoft.Extensions.Options.IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter the JWT in the format: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });            

            var app = builder.Build();                   

            app.UseHttpsRedirection();
            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{

            var provider = app.Services.GetRequiredService<Asp.Versioning.ApiExplorer.IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"Cart API {description.GroupName.ToUpperInvariant()}");
                }
                //options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cart API v1");
                //options.SwaggerEndpoint("/swagger/v2/swagger.json", "Cart API v2");
            });
            //}

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<TokenLoggingMiddleware>();
            app.MapControllers();
            app.Run();
        }
    }
}
