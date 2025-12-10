using AutoMapper;
using DTO_s;
using Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    internal class AutoMapper: Profile
    {
        public AutoMapper() 
        {
            CreateMap<User, UserDTO>();
            CreateMap<Order, OrdersDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>();
            CreateMap<Category, CategroryDTO>();
        }
    }
}
