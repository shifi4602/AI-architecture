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
    public class CategoryRepository : ICategoryRepository
    {
        ApiShopContext _apiShopContext;
        public CategoryRepository(ApiShopContext apiShopContext)
        {
            _apiShopContext = apiShopContext;
        }
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _apiShopContext.Categories.ToListAsync();
        }
    }
}
