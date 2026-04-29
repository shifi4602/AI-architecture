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
    public class ProtuctService : IProtuctService
    {
        IProductReposetory _iProductReposetory;
        IMapper _mapper;
        public ProtuctService(IProductReposetory iProductReposetory, IMapper imapper)
        {
            _iProductReposetory = iProductReposetory;
            _mapper = imapper;
        }
        public async Task<ProductRespone<ProductDTO>> GetProducts(int position, int skip, string? name, string description, int[]? categories, int? minPrice, int? maxPrice, string? orderBy)
        {
            List<Product> products;
            ProductRespone<ProductDTO> pageResponse = new ProductRespone<ProductDTO>();
            (products, pageResponse.TotalItems) = await _iProductReposetory.GetProducts(position, skip, name, description, categories, minPrice, maxPrice, orderBy);
            pageResponse.Data = _mapper.Map<List<Product>, List<ProductDTO>>(products);
            pageResponse.CurrentPage = position;
            pageResponse.HasPreviousPage = pageResponse.CurrentPage > 1;
            pageResponse.HasNextPage = (pageResponse.TotalItems / skip) > (pageResponse.CurrentPage - 1);
            pageResponse.PageSize = skip;
            return pageResponse;
        }

        public async Task<ProductDTO?> GetProductById(int id)
        {
            var product = await _iProductReposetory.GetProductById(id);
            if (product == null) return null;
            return _mapper.Map<ProductDTO>(product);
        }
    }
}
