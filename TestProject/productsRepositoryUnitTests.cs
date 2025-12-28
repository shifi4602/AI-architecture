using Enteties;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class productsRepositoryUnitTests
    {
        private readonly Mock<ApiShopContext> _mockContext;
        private readonly ProductReposetory _repository;

        public productsRepositoryUnitTests()
        {
            _mockContext = new Mock<ApiShopContext>(new DbContextOptions<ApiShopContext>());
            _repository = new ProductReposetory(_mockContext.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsProductList_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductsId = 1, ProductName = "Product A", Price = 10 },
                new Product { ProductsId = 2, ProductName = "Product B", Price = 20 }
            };

            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.AsQueryable().Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            _mockContext.Setup(m => m.Products).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetProducts(null, null, null, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetProducts_ReturnsEmptyList_WhenNoProductsExist()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(Enumerable.Empty<Product>().AsQueryable().Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(Enumerable.Empty<Product>().AsQueryable().Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(Enumerable.Empty<Product>().AsQueryable().ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(Enumerable.Empty<Product>().GetEnumerator());

            _mockContext.Setup(m => m.Products).Returns(mockSet.Object);

            // Act
            var result = await _repository.GetProducts(null, null, null, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        //[Fact]
        //public async Task GetProducts_Throws_WhenFetchingProductsFails()
        //{
        //    // Arrange: mock a context that throws an exception when fetching products
        //    var throwingContext = new Mock<ApiShopContext>(new DbContextOptions<ApiShopContext>());
        //    throwingContext.Setup(m => m.Products).Throws(new InvalidOperationException("Simulated database exception"));
        //    var repo = new ProductReposetory(throwingContext.Object);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<InvalidOperationException>(() => repo.GetProducts(null, null, null, null, null, null, null));
        //}
    }
}
