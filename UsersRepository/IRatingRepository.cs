using Enteties;

namespace Repositories
{
    public interface IRatingRepository
    {
        Task<Rating> AddRating(Rating newRating);
    }
}