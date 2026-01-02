using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Enteties;
using Xunit;

namespace TestProject
{
    // Integration-style tests against EF Core InMemory provider.
    // IAsyncLifetime provides async per-test setup/teardown hooks.
    public class OrdersRepositoryIntegrationTests //: IAsyncLifetime
    {
        private readonly ApiShopContext _context;
        private readonly IOrdersRepository _repository;

        // Setup: Initialize in-memory database before each test
        public OrdersRepositoryIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApiShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApiShopContext(options);  // Create a fresh context for each test
            _repository = new OrdersRepository(_context);  // Inject the context into the repository
        }

        // Cleanup: Ensure the database is cleaned after each test
        public void Dispose()
        {
            _context.Database.EnsureDeleted();  // Delete the in-memory database
            _context.Dispose();  // Dispose the context
        }

        // Test for AddOrder method
        [Fact]
        public async Task AddOrder_ShouldAddOrder()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 1,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2 },
                    new OrderItem { ProductId = 2, Quantity = 3 }
                }
            };

            // Act
            var result = await _repository.AddOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            Assert.Equal(2, result.OrderItems.Count);

            // Verify the order is in the database
            var dbOrder = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            Assert.NotNull(dbOrder);
            Assert.Equal(order.OrderId, dbOrder.OrderId);
            Assert.Equal(2, dbOrder.OrderItems.Count);
        }

        // Test for GetOrderById method (order exists)
        [Fact]
        public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 2,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2 }
                }
            };
            await _repository.AddOrder(order);  // Add order to DB

            // Act
            var result = await _repository.GetOrderById(order.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderId, result.OrderId);
            Assert.Single(result.OrderItems);
        }

        // Test for GetOrderById method (order does not exist)
        [Fact]
        public async Task GetOrderById_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Act
            var result = await _repository.GetOrderById(999);  // Non-existing order ID

            // Assert
            Assert.Null(result);
        }

        // Test for AddOrder should save data in database
        [Fact]
        public async Task AddOrder_ShouldSaveDataInDatabase()
        {
            // Arrange
            var order = new Order
            {
                OrderId = 3,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 3, Quantity = 1 }
                }
            };

            // Act
            await _repository.AddOrder(order);

            // Assert
            var savedOrder = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            Assert.NotNull(savedOrder);
            Assert.Equal(order.OrderId, savedOrder.OrderId);
            Assert.Single(savedOrder.OrderItems);
        }
    }
}