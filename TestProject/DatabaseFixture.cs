using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class DatabaseFixture
    {
        public ApiShopContext Context { get; private set; }
        public DatabaseFixture()
        {
            // Set up the test database connection and initialize the context
            var options = new DbContextOptionsBuilder<ApiShopContext>()

                .UseSqlServer($"Server=(localdb)\\MSSQLLocalDB;Database=ApiShop_Test_{Guid.NewGuid()};Trusted_Connection=True;")
                .Options;
            Context = new ApiShopContext(options);
            Context.Database.EnsureCreated();
        }
        public void Dispose()
        {
            // Clean up the test database after all tests are completed
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
