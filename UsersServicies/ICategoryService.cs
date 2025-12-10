using DTO_s;
using Enteties;

namespace Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategroryDTO>> GetCategories();
    }
}