using DTO_s;
using Enteties;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Text.Json;

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
        public async Task<ActionResult<ProductRespone<ProductDTO>>> Get(int position = 1, int skip = 10, string? name = null, [FromQuery] int[]? categoryIds = null, string? description = null, int? maxPrice = null, int? minPrice = null, string? orderBy = null)
        {
            var response = await _iProtuctService.GetProducts(position, skip, name, description ?? string.Empty, categoryIds, minPrice, maxPrice, orderBy);
            return Ok(response);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            ProductDTO? product = await _iProtuctService.GetProductById(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [AuthorizeRoles(AppRoles.Admin)]
        public async Task<ActionResult<ProductDTO>> Post([FromBody] ProductDTO productDTO)
        {
            var created = await _iProtuctService.AddProduct(productDTO);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductsId }, created);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        [AuthorizeRoles(AppRoles.Admin)]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDTO productDTO)
        {
            var updated = await _iProtuctService.UpdateProduct(id, productDTO);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        [AuthorizeRoles(AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _iProtuctService.DeleteProduct(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
