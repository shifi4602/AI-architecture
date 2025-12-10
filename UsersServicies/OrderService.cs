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
    public class OrderService : IOrderService
    {
        IOrdersRepository _iOrdersRepository;
        IMapper _mapper;
        public OrderService(IOrdersRepository iOrdersRepository, IMapper mapper)
        {
            _iOrdersRepository = iOrdersRepository;
            _mapper = mapper;
        }

        public async Task<OrdersDTO> GetOrderById(int id)
        {
            Order order = await _iOrdersRepository.GetOrderById(id);
            OrdersDTO orderDto = _mapper.Map<Order, OrdersDTO>(order);
            return orderDto;
        }
        public async Task<OrdersDTO> AddNewOrder(OrdersDTO orderDto)
        {
            Order order = _mapper.Map<OrdersDTO, Order>(orderDto);
            Order order1 = await _iOrdersRepository.AddOrder(order);
            OrdersDTO orderDto1 = _mapper.Map<Order, OrdersDTO>(order1);
            return orderDto1;
        }
    }
}
