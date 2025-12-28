using Enteties;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Repositories.Tests
{
    // Integration-style tests against EF Core InMemory provider.
    // IAsyncLifetime provides async per-test setup/teardown hooks.
    public class OrdersRepositoryUnitTests //: IAsyncLifetime
    {
        private readonly Mock<ApiShopContext> _mockContext;
        private readonly IOrdersRepository _repository;
        private readonly Mock<DbSet<Order>> _mockOrderSet;
        private readonly Mock<DbSet<OrderItem>> _mockOrderItemSet;

        public OrdersRepositoryUnitTests()
        {
            _mockContext = new Mock<ApiShopContext>();
            _mockOrderSet = new Mock<DbSet<Order>>();
            _mockOrderItemSet = new Mock<DbSet<OrderItem>>();

            // Mock DbSet behavior
            _mockContext.Setup(m => m.Orders).Returns(_mockOrderSet.Object);
            _mockContext.Setup(m => m.OrderItems).Returns(_mockOrderItemSet.Object);

            _repository = new OrdersRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrder_WithOrderItems_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                OrderId = orderId,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2 },
                    new OrderItem { ProductId = 2, Quantity = 1 }
                }
            };

            var ordersData = new List<Order> { order }.AsQueryable();
            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(ordersData.Provider);
            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(ordersData.Expression);
            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(ordersData.ElementType);
            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(ordersData.GetEnumerator());

            // Act
            var result = await _repository.GetOrderById(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(2, result.OrderItems.Count);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 999; // Non-existing order ID

            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(Enumerable.Empty<Order>().AsQueryable().Provider);
            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(Enumerable.Empty<Order>().AsQueryable().Expression);
            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<Order>().AsQueryable().ElementType);
            _mockOrderSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<Order>().AsQueryable().GetEnumerator());

            // Act
            var result = await _repository.GetOrderById(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddOrder_ReturnsOrder_WhenOrderIsAdded()
        {
            // Arrange
            var order = new Order { OrderId = 1, OrderItems = new List<OrderItem>() };

            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1); // Simulate save changes

            // Act
            var result = await _repository.AddOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once); // Verify SaveChangesAsync was called
        }

        [Fact]
        public async Task AddOrder_ShouldCallAddAsync_WhenOrderIsAdded()
        {
            // Arrange
            var order = new Order { OrderId = 1, OrderItems = new List<OrderItem>() };

            // Act
            await _repository.AddOrder(order);

            // Assert
            _mockOrderSet.Verify(m => m.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}