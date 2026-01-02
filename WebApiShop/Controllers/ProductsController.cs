using DTO_s;
using Enteties;
using MailKit.Search;
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
        public async Task<ActionResult<ProductRespone<ProductDTO>>> Get(int position, int skip, string? name,  [FromQuery] int[]? categoryIds, string? description, int? maxPrice, int? minPrice, string? orderBy)
        {
            //ProductRespone<ProductDTO> pageResponse = await _iProtuctService.GetProducts(position, skip, name, description, categoryIds, minPrice, maxPrice, orderBy);
            //if (pageResponse.Data.Count() > 0)
            //    return Ok(pageResponse);
            //return NoContent();
            return await _iProtuctService.GetProducts(position, skip, name, description, categoryIds, minPrice, maxPrice, orderBy);
        }
    }
}
