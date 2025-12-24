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
        public async Task<IEnumerable<ProductDTO>> GetProducts(string? name, int[]? categories, int? minPrice, int? maxPrice, int? limit, string? orderBy, int? offset)
        {
            //return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(await _iProductReposetory.GetProducts(name, categories, minPrice, maxPrice, limit, orderBy, offset));
            IEnumerable<Product> products = await _iProductReposetory.GetProducts(name, categories, minPrice, maxPrice, limit, orderBy, offset);
            IEnumerable<ProductDTO> productDTOs = _mapper.Map<IEnumerable<Product>,IEnumerable<ProductDTO>>(products);
            return productDTOs;
        }
    }
}
