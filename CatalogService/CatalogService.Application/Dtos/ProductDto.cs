namespace CatalogService.Application.Dtos
{
    public record ProductDto(Guid Id, string Name, string? Description, string? ImageUrl, Guid CategoryId, decimal Price, int Amount);
}
