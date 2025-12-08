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
        public OrderService(IOrdersRepository iOrdersRepository)
        {
            _iOrdersRepository = iOrdersRepository;
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _iOrdersRepository.GetOrderById(id);
        }
        public async Task<Order> AddNewOrder(Order order)
        {
            return await _iOrdersRepository.AddOrder(order);
        }
    }
}
