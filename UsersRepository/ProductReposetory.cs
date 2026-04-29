using Enteties;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProductReposetory : IProductReposetory
    {
        ApiShopContext _apiShopContext;
        public ProductReposetory(ApiShopContext apiShopContext)
        {
            _apiShopContext = apiShopContext;
        }

        public async Task<(List<Product> Items, int TotalCount)> GetProducts(int position, int skip, string? name, string? description, int[]? categories, int? nimPrice, int? maxPrice, string? orderBy)
        {
            position = position < 1 ? 1 : position;
            skip = skip < 1 ? 10 : skip;
            var query = _apiShopContext.Products.Where(product =>
            (description == null ? (true) : product.Description.Contains(description)) &&
            (name == null ? (true) : product.ProductName.Contains(name)) &&
            (categories == null ||categories.Length == 0 || categories.Length == 0 ? (true) : categories.Contains(product.CategoryId)) &&
            (nimPrice == null ? (true) : product.Price >= nimPrice) &&
            (maxPrice == null ? (true) : product.Price <= maxPrice)
            );
            if (orderBy != null)
            {
                switch (orderBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(p => p.ProductName);
                        break;
                    case "price_asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    default:
                        query = query.OrderBy(p => p.ProductName);
                        break;
                }
            }

            Console.WriteLine(query.ToQueryString());
            var total = await query.CountAsync();
            var products = await query.Skip((position - 1) * skip)
                .Take(skip).Include(p => p.Category).ToListAsync();
            return (products, total);
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _apiShopContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductsId == id);
        }
    }
}
