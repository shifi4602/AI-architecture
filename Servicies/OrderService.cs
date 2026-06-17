using AutoMapper;
using DTO_s;
using Enteties;
using Microsoft.Extensions.Logging;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        IOrdersRepository _iOrdersRepository;
        IProductReposetory _iProductReposetory;
        IKafkaProducerService _kafkaProducerService;
        ILogger<OrderService> _logger;
        IMapper _mapper;
        public OrderService(IOrdersRepository iOrdersRepository, IProductReposetory iProductReposetory, IKafkaProducerService kafkaProducerService, ILogger<OrderService> logger, IMapper mapper)
        {
            _iOrdersRepository = iOrdersRepository;
            _iProductReposetory = iProductReposetory;
            _kafkaProducerService = kafkaProducerService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OrdersDTO> GetOrderById(int id)
        {
            Order order = await _iOrdersRepository.GetOrderById(id);
            OrdersDTO orderDTO = _mapper.Map<Order, OrdersDTO>(order);
            return orderDTO;
        }
        public async Task<OrdersDTO> AddNewOrder(OrdersDTO orderDTO)
        {
            Order order = _mapper.Map<OrdersDTO, Order>(orderDTO);
            Order orderRes = await _iOrdersRepository.AddOrder(order);
            OrdersDTO orderDtoRes = _mapper.Map<Order, OrdersDTO>(orderRes);

            var orderCreatedEvent = new
            {
                EventType = "OrderCreated",
                orderDtoRes.OrderId,
                orderDtoRes.UserId,
                orderDtoRes.OrderSum,
                orderDtoRes.OrderDate,
                CreatedAtUtc = DateTime.UtcNow
            };

            try
            {
                string payload = JsonSerializer.Serialize(orderCreatedEvent);
                await _kafkaProducerService.ProduceAsync(orderDtoRes.OrderId.ToString(), payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish OrderCreated event for order id {OrderId}", orderDtoRes.OrderId);
            }

            return orderDtoRes;
        }

        public async Task<bool> ValidateOrderSum(OrdersDTO orderDTO)
        {
            decimal expectedSum = 0;
            foreach (var item in orderDTO.OrderItems)
            {
                Product? product = await _iProductReposetory.GetProductById(item.ProductId);
                if (product == null)
                    return false;
                expectedSum += product.Price * item.Quantity;
            }
            return (int)expectedSum == orderDTO.OrderSum;
        }
    }
}
