using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Enteties;
using Xunit;

namespace Repositories.Tests
{
    public class CategoryRepositoryTests //: IDisposable
    {
        private readonly ApiShopContext _context;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApiShopContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApiShopContext(options);
            _context.Database.EnsureCreated();

            _repository = new CategoryRepository(_context);
        }

        [Fact]
        public async Task GetCategories_ReturnsAll_WhenCategoriesExist()
        {
            // Arrange
            var categories = new[]
            {
                new Category { CategoryName = "Cat A" },
                new Category { CategoryName = "Cat B" }
            };
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCategories();

            // Assert
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, c => c.CategoryName == "Cat A");
            Assert.Contains(list, c => c.CategoryName == "Cat B");
        }

        [Fact]
        public async Task GetCategories_ReturnsEmpty_WhenNoCategoriesExist()
        {
            // Ensure DB is empty for categories
            var result = await _repository.GetCategories();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCategories_Throws_WhenDatabaseAccessFails()
        {
            // Arrange: use a context whose Categories getter throws to simulate DB failure
            var options = new DbContextOptionsBuilder<ApiShopContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            await using var throwingContext = new ThrowingApiShopContext(options);
            var repo = new CategoryRepository(throwingContext);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repo.GetCategories());
        }

        // Derived context that simulates a failure when Categories is accessed
        private sealed class ThrowingApiShopContext : ApiShopContext
        {
            public ThrowingApiShopContext(DbContextOptions<ApiShopContext> options) : base(options) { }

            public override DbSet<Category> Categories
            {
                get => throw new InvalidOperationException("Simulated DB access failure");
                set => base.Categories = value;
            }
        }
    }
}