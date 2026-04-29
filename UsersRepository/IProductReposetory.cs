using Enteties;

namespace Repositories
{
    public interface IProductReposetory
    {
        Task<(List<Product> Items, int TotalCount)> GetProducts(int position, int skip, string? name, string? description, int[]? categories, int? nimPrice, int? maxPrice, string? orderBy);
        Task<Product?> GetProductById(int id);
    }
}