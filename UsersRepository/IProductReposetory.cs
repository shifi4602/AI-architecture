using Enteties;

namespace Repositories
{
    public interface IProductReposetory
    {
        Task<IEnumerable<Product>> GetProducts(string? name, int[]? categories, int? nimPrice, int? maxPrice, int? limit, string? orderBy, int? offset);
    }
}