using Enteties;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProtuctService _iProtuctService;
        public ProductsController(IProtuctService iProtuctService)
        {
            _iProtuctService = iProtuctService;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<IEnumerable<Product>> GetProducts(string? name, int[]? categories, int? minPrice, int? maxPrice, int? limit, string? orderBy, int? offset)
        {
            return await _iProtuctService.GetProducts(name, categories, minPrice,maxPrice, limit, orderBy, offset);
        }
    }
}
