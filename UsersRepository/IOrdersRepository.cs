using Enteties;

namespace Repositories
{
    public interface IOrdersRepository
    {
        Task<Order> AddOrder(Order order);
        Task<Order> GetOrderById(int id);
    }
}