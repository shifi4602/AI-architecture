using DTO_s;
using Enteties;

namespace Services
{
    public interface IProtuctService
    {
        Task<ProductRespone<ProductDTO>> GetProducts(int position, int skip, string? name, string description, int[]? categories, int? minPrice, int? maxPrice, string? orderBy);
        Task<ProductDTO?> GetProductById(int id);
        Task<ProductDTO> AddProduct(ProductDTO productDTO);
        Task<bool> UpdateProduct(int id, ProductDTO productDTO);
        Task<bool> DeleteProduct(int id);
    }
}