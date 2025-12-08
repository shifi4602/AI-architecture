using Enteties;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProtuctService : IProtuctService
    {
        IProductReposetory _iProductReposetory;
        public ProtuctService(IProductReposetory iProductReposetory)
        {
            _iProductReposetory = iProductReposetory;
        }
        public async Task<IEnumerable<Product>> GetProducts(string? name, int[]? categories, int? minPrice, int? maxPrice, int? limit, string? orderBy, int? offset)
        {
            return await _iProductReposetory.GetProducts(name, categories, minPrice, maxPrice, limit, orderBy, offset);
        }
    }
}
