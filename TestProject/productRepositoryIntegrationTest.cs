using Enteties;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class productRepositoryIntegrationTest
    {
        private readonly DatabaseFixture _fixture;
        private readonly ApiShopContext _dbContext;
        private readonly IProductReposetory _productRepository;

        public productRepositoryIntegrationTest()
        {
            _fixture = new DatabaseFixture();
            _dbContext = _fixture.Context;
            _productRepository = new ProductReposetory(_dbContext);
        }
        public void Dispose()
        {
            _fixture.Dispose();
        }

        [Fact]
        public async Task GetProducts_WhenProductsExist_ReturnsAllProductsWithCategory()
        {
            // Arrange
            var category = new Category { CategoryName = "Electronics" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var testProducts = new List<Product>
            {
                new Product { ProductName = "Laptop", CategoryId = category.CategoryId, Price = 3500 },
                new Product { ProductName = "Mouse", CategoryId = category.CategoryId, Price = 150 }
            };

            await _dbContext.Products.AddRangeAsync(testProducts);
            await _dbContext.SaveChangesAsync();

            // Act
            var (result, _) = await _productRepository.GetProducts(
                position: 0,
                skip: 10,
                name: null,
                description: null,
                categories: null,
                nimPrice: null,
                maxPrice: null,
                orderBy: null
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.NotNull(p.Category));
            Assert.Contains(result, p => p.ProductName == "Laptop" && p.Category.CategoryName == "Electronics");
        }

        [Fact]
        public async Task GetProducts_WhenNoProductsExist_ReturnsEmptyList()
        {
            // Act
            var (result, _) = await _productRepository.GetProducts(
                position: 0,
                skip: 10,
                name: null,
                description: null,
                categories: null,
                nimPrice: null,
                maxPrice: null,
                orderBy: null
            );

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
