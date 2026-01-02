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
    public class ProductReposoryUnitTest
    {
        private readonly ApiShopContext _context;
        private readonly IProductReposetory _repository;

        public ProductReposoryUnitTest()
        {
            var options = new DbContextOptionsBuilder<ApiShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique in-memory DB for each test
                .Options;

            _context = new ApiShopContext(options); // Use _context as the class variable
            _repository = new ProductReposetory(_context); // Use _repository as the class variable
        }
        private ApiShopContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApiShopContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApiShopContext(options);

            context.Categories.AddRange(
                new Category { CategoryId = 1, CategoryName = "Cat1" },
                new Category { CategoryId = 2, CategoryName = "Cat2" }
            );

            context.Products.AddRange(
                new Product
                {
                    ProductsId = 1,
                    ProductName = "Phone",
                    Description = "Smart",
                    Price = 1000,
                    CategoryId = 1
                },
                new Product
                {
                    ProductsId = 2,
                    ProductName = "Laptop",
                    Description = "Gaming",
                    Price = 5000,
                    CategoryId = 1
                },
                new Product
                {
                    ProductsId = 3,
                    ProductName = "Book",
                    Description = "Programming",
                    Price = 200,
                    CategoryId = 2
                }
            );

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetProducts_NoFilters_ReturnsAll()
        {
            var context = CreateContext();
            var repo = new ProductReposetory(context);

            var result = await repo.GetProducts(
                position: 1,
                skip: 10,
                name: null,
                description: null,
                categories: null,
                nimPrice: null,
                maxPrice: null,
                orderBy: null
            );

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Items.Count);
        }

        [Fact]
        public async Task GetProducts_FilterByName()
        {
            var context = CreateContext();
            var repo = new ProductReposetory(context);

            var result = await repo.GetProducts(
                position: 1,
                skip: 10,
                name: "Phone",
                description: null,
                categories: null,
                nimPrice: null,
                maxPrice: null,
                orderBy: null
            );

            Assert.Single(result.Items);
            Assert.Equal("Phone", result.Items.First().ProductName);
        }

        [Fact]
        public async Task GetProducts_FilterByCategory()
        {
            var context = CreateContext();
            var repo = new ProductReposetory(context);

            var result = await repo.GetProducts(
                position: 1,
                skip: 10,
                name: null,
                description: null,
                categories: new[] { 2 },
                nimPrice: null,
                maxPrice: null,
                orderBy: null
            );

            Assert.Single(result.Items);
            Assert.Equal(2, result.Items.First().CategoryId);
        }

        [Fact]
        public async Task GetProducts_FilterByPriceRange()
        {
            var context = CreateContext();
            var repo = new ProductReposetory(context);

            var result = await repo.GetProducts(
                position: 1,
                skip: 10,
                name: null,
                description: null,
                categories: null,
                nimPrice: 300,
                maxPrice: 2000,
                orderBy: null
            );

            Assert.Single(result.Items);
            Assert.Equal("Phone", result.Items.First().ProductName);
        }

        [Fact]
        public async Task GetProducts_OrderByPriceDesc()
        {
            var context = CreateContext();
            var repo = new ProductReposetory(context);

            var result = await repo.GetProducts(
                position: 1,
                skip: 10,
                name: null,
                description: null,
                categories: null,
                nimPrice: null,
                maxPrice: null,
                orderBy: "price_desc"
            );

            Assert.Equal(5000, result.Items.First().Price);
        }

        [Fact]
        public async Task GetProducts_Pagination_WorksCorrectly()
        {
            var context = CreateContext();
            var repo = new ProductReposetory(context);

            var result = await repo.GetProducts(
                position: 2,
                skip: 1,
                name: null,
                description: null,
                categories: null,
                nimPrice: null,
                maxPrice: null,
                orderBy: "name"
            );

            Assert.Single(result.Items);
            Assert.Equal(3, result.TotalCount);
        }
    }
}
