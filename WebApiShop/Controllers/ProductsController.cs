using DTO_s;
using Enteties;
using MailKit.Search;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public ProductsController(IProtuctService iProtuctService, IDistributedCache cache, IConfiguration configuration)
        {
            _iProtuctService = iProtuctService;
            _cache = cache;
            _configuration = configuration;
        }
        // GET: api/<ProductsController>
        [HttpGet]
        public async Task<ActionResult<ProductRespone<ProductDTO>>> Get(int position, int skip, string? name,  [FromQuery] int[]? categoryIds, string? description, int? maxPrice, int? minPrice, string? orderBy)
        {
            var cacheKey = BuildProductsCacheKey(position, skip, name, categoryIds, description, maxPrice, minPrice, orderBy);
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            // Cache hit: return directly from Redis.
            if (!string.IsNullOrWhiteSpace(cachedValue))
            {
                var cachedResponse = JsonSerializer.Deserialize<ProductRespone<ProductDTO>>(cachedValue);
                if (cachedResponse != null)
                {
                    return Ok(cachedResponse);
                }
            }

            // Cache miss: read from DB/service layer.
            var response = await _iProtuctService.GetProducts(position, skip, name, description ?? string.Empty, categoryIds, minPrice, maxPrice, orderBy);

            var ttlMinutes = _configuration.GetValue<int?>("RedisCache:DefaultTtlMinutes") ?? 4;
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes)
            };

            var serializedResponse = JsonSerializer.Serialize(response);
            await _cache.SetStringAsync(cacheKey, serializedResponse, cacheOptions);

            return Ok(response);
        }

        private static string BuildProductsCacheKey(int? position, int? skip, string? name, int[]? categoryIds, string? description, int? maxPrice, int? minPrice, string? orderBy)
        {
            var orderedCategories = categoryIds == null || categoryIds.Length == 0
                ? "none"
                : string.Join(",", categoryIds.OrderBy(x => x));

            return $"products:p={position ?? 1}:s={skip ?? 6}:n={name ?? string.Empty}:c={orderedCategories}:d={description ?? string.Empty}:max={maxPrice?.ToString() ?? string.Empty}:min={minPrice?.ToString() ?? string.Empty}:o={orderBy ?? string.Empty}";
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var cacheKey = $"product:id={id}";
            var cachedValue = await _cache.GetStringAsync(cacheKey);

            // Cache hit: return directly from Redis.
            if (!string.IsNullOrWhiteSpace(cachedValue))
            {
                var cachedProduct = JsonSerializer.Deserialize<ProductDTO>(cachedValue);
                if (cachedProduct != null)
                {
                    return Ok(cachedProduct);
                }
            }

            // Cache miss: read from DB/service layer.
            ProductDTO product = await _iProtuctService.GetProductById(id);
            if (product == null)
                return NotFound();

            var ttlMinutes = _configuration.GetValue<int?>("RedisCache:DefaultTtlMinutes") ?? 4;
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes)
            };

            var serializedProduct = JsonSerializer.Serialize(product);
            await _cache.SetStringAsync(cacheKey, serializedProduct, cacheOptions);

            return Ok(product);
        }
    }
}
