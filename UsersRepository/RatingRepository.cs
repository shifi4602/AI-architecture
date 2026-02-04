using Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApiShopContext _apiShopContext;
        public RatingRepository(ApiShopContext apiShopContext)
        {
            _apiShopContext = apiShopContext;
        }
        public async Task<Rating> AddRating(Rating newRating)
        {
            await _apiShopContext.Ratings.AddAsync(newRating);
            await _apiShopContext.SaveChangesAsync();
            return newRating;
        }
    }
}

