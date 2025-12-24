using DTO_s;
using Enteties;

namespace Services
{
    public interface IProtuctService
    {
        Task<IEnumerable<ProductDTO>> GetProducts(string? name, int[]? categories, int? minPrice, int? maxPrice, int? limit, string? orderBy, int? offset);
    }
}