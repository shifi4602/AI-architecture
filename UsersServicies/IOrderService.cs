using DTO_s;
using Enteties;

namespace Services
{
    public interface IOrderService
    {
        Task<OrdersDTO> AddNewOrder(OrdersDTO orderDTO);
        Task<OrdersDTO> GetOrderById(int id);
    }
}