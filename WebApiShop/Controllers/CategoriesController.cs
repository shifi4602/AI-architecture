using DTO_s;
using Enteties;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        ICategoryService _iCategoryService;

        public CategoriesController(ICategoryService iCategoryService)
        {
            _iCategoryService = iCategoryService;
        }
        // GET: api/<CategoriesController>
        [HttpGet]
        public async Task<IEnumerable<CategroryDTO>> GetCategories()
        {
            return await _iCategoryService.GetCategories();
        }
    }
}
