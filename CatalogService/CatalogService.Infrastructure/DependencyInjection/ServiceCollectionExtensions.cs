using CatalogService.Application.Messaging;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Messaging;
using CatalogService.Infrastructure.Persistence;
using CatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CatalogDbContext>(opt =>
                opt.UseSqlServer(connectionString));

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

            return services;
        }
    }
}
