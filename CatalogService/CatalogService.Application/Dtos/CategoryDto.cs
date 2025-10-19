namespace CatalogService.Application.Dtos
{
    public record CategoryDto(Guid Id, string Name, string? ImageUrl, Guid? ParentCategoryId);
}
