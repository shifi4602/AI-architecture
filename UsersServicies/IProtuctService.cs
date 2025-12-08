using Enteties;

namespace Services
{
    public interface IProtuctService
    {
        Task<IEnumerable<Product>> GetProducts(string? name, int[]? categories, int? minPrice, int? maxPrice, int? limit, string? orderBy, int? offset);
    }
}