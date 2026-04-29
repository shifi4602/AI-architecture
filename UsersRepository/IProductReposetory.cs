using Enteties;

namespace Repositories
{
    public interface IProductReposetory
    {
        Task<(List<Product> Items, int TotalCount)> GetProducts(int position, int skip, string? name, string? description, int[]? categories, int? nimPrice, int? maxPrice, string? orderBy);
        Task<Product?> GetProductById(int id);
        Task<Product> AddProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);
    }
}