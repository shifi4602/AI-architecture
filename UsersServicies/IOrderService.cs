using DTO_s;
using Enteties;

namespace Services
{
    public interface IOrderService
    {
        Task<OrdersDTO> AddNewOrder(OrdersDTO orderDto);
        Task<OrdersDTO> GetOrderById(int id);
    }
}