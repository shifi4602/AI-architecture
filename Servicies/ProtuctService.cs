using AutoMapper;
using DTO_s;
using Enteties;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Services
{
    public class RedisCacheSettingsOptions
    {
        public int DefaultTtlMinutes { get; set; } = 4;
    }

    public class ProtuctService : IProtuctService
    {
        IProductReposetory _iProductReposetory;
        IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly int _ttlMinutes;

        public ProtuctService(IProductReposetory iProductReposetory, IMapper imapper,
            IDistributedCache cache, IConnectionMultiplexer redis,
            IOptions<RedisCacheSettingsOptions> cacheSettings)
        {
            _iProductReposetory = iProductReposetory;
            _mapper = imapper;
            _cache = cache;
            _redis = redis;
            _ttlMinutes = cacheSettings.Value.DefaultTtlMinutes;
        }

        private static string BuildProductsCacheKey(int position, int skip, string? name, int[]? categoryIds, string? description, int? maxPrice, int? minPrice, string? orderBy)
        {
            var orderedCategories = categoryIds == null || categoryIds.Length == 0
                ? "none"
                : string.Join(",", categoryIds.OrderBy(x => x));
            return $"products:p={position}:s={skip}:n={name ?? string.Empty}:c={orderedCategories}:d={description ?? string.Empty}:max={maxPrice?.ToString() ?? string.Empty}:min={minPrice?.ToString() ?? string.Empty}:o={orderBy ?? string.Empty}";
        }

        private async Task InvalidateProductListCacheAsync()
        {
            var db = _redis.GetDatabase();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: "WebApiShop:products:*").ToArray();
            if (keys.Length > 0)
                await db.KeyDeleteAsync(keys);
        }

        public async Task<ProductRespone<ProductDTO>> GetProducts(int position, int skip, string? name, string description, int[]? categories, int? minPrice, int? maxPrice, string? orderBy)
        {
            var cacheKey = BuildProductsCacheKey(position, skip, name, categories, description, maxPrice, minPrice, orderBy);
            var cachedValue = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cachedValue))
            {
                var cached = JsonSerializer.Deserialize<ProductRespone<ProductDTO>>(cachedValue);
                if (cached != null) return cached;
            }

            List<Product> products;
            ProductRespone<ProductDTO> pageResponse = new();
            (products, pageResponse.TotalItems) = await _iProductReposetory.GetProducts(position, skip, name, description, categories, minPrice, maxPrice, orderBy);
            pageResponse.Data = _mapper.Map<List<Product>, List<ProductDTO>>(products);
            pageResponse.CurrentPage = position;
            pageResponse.HasPreviousPage = pageResponse.CurrentPage > 1;
            pageResponse.HasNextPage = (pageResponse.TotalItems / skip) > (pageResponse.CurrentPage - 1);
            pageResponse.PageSize = skip;

            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_ttlMinutes) };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(pageResponse), cacheOptions);
            return pageResponse;
        }

        public async Task<ProductDTO?> GetProductById(int id)
        {
            var cacheKey = $"product:id={id}";
            var cachedValue = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrWhiteSpace(cachedValue))
            {
                var cached = JsonSerializer.Deserialize<ProductDTO>(cachedValue);
                if (cached != null) return cached;
            }

            var product = await _iProductReposetory.GetProductById(id);
            if (product == null) return null;
            var dto = _mapper.Map<ProductDTO>(product);

            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_ttlMinutes) };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);
            return dto;
        }

        public async Task<ProductDTO> AddProduct(ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            var result = await _iProductReposetory.AddProduct(product);
            await InvalidateProductListCacheAsync();
            return _mapper.Map<ProductDTO>(result);
        }

        public async Task<bool> UpdateProduct(int id, ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            product.ProductsId = id;
            var updated = await _iProductReposetory.UpdateProduct(product);
            if (updated)
            {
                await _cache.RemoveAsync($"product:id={id}");
                await InvalidateProductListCacheAsync();
            }
            return updated;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var deleted = await _iProductReposetory.DeleteProduct(id);
            if (deleted)
            {
                await _cache.RemoveAsync($"product:id={id}");
                await InvalidateProductListCacheAsync();
            }
            return deleted;
        }
    }
}
