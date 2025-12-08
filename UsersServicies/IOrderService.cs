using Enteties;

namespace Services
{
    public interface IOrderService
    {
        Task<Order> AddNewOrder(Order order);
        Task<Order> GetOrderById(int id);
    }
}