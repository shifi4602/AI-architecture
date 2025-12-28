using Enteties;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        ApiShopContext _apiShopContext;
        public OrdersRepository(ApiShopContext apiShopContext)
        {
            _apiShopContext = apiShopContext;
        }
        //repository
        public async Task<Order> GetOrderById(int id)
        {
            return await _apiShopContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(oo => oo.OrderId == id);
        }
        public async Task<Order> AddOrder(Order order)
        {
            await _apiShopContext.Orders.AddAsync(order);
            await _apiShopContext.SaveChangesAsync();
            //return await _apiShopContext.FindAsync<Order>(order.OrderId);
            return order;
        }
    }
}
