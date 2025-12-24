using Enteties;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
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

        public async Task<List<Product>> GetProducts(string? name, int[]? categories, int? nimPrice, int? maxPrice, int? limit, string? orderBy, int? offset)
        {
            return await _apiShopContext.Products.ToListAsync();
        }
    }
}
