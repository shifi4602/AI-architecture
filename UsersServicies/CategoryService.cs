using AutoMapper;
using DTO_s;
using Enteties;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        IMapper _mapper;
        ICategoryRepository _iCategoryRepository;
        public CategoryService(ICategoryRepository iCategoryRepository, IMapper mapper)
        {
            _iCategoryRepository = iCategoryRepository;
            _mapper = mapper;   
        }
        public async Task<IEnumerable<CategroryDTO>> GetCategories()
        {
            IEnumerable<Category> categories = await _iCategoryRepository.GetCategories();
            IEnumerable<CategroryDTO> categroryDTOs = _mapper.Map<IEnumerable<Category>, IEnumerable<CategroryDTO>>(categories);
            return categroryDTOs;
        }
    }
}
