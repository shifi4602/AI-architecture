using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Enteties;

namespace TestProject
{
    // Each test gets a fresh test-class instance in xUnit,
    // so IAsyncLifetime runs before/after each test (per-test setup/teardown).
    public class UserRepositoryIntegrationTests //: IAsyncLifetime
    {
        private ApiShopContext _context;
        private IUsersRepository _repository;

        // SetUp: Initialize the in-memory database and repository before each test
        public UserRepositoryIntegrationTests()
        {
            // Create a new in-memory database for each test
            var options = new DbContextOptionsBuilder<ApiShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique name for each test run
                .Options;

            _context = new ApiShopContext(options);  // Create a fresh context
            _repository = new UsersRepository(_context);  // Inject it into the repository
        }

        // TearDown: Clean up the in-memory database after each test
        public void Dispose()
        {
            if(_context != null)
            {
                _context.Database.EnsureDeleted();  // Deletes the database after each test
                _context.Dispose();  // Disposes the context
            }
        }

        // Test for AddUser method
        [Fact]
        public async Task AddUser_ShouldAddUser()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", Password = "password1", FirstName = "a", LastName = "b" };

            // Act
            var addedUser = await _repository.AddUser(user);

            // Assert
            Assert.NotNull(addedUser);
            Assert.Equal("user1@test.com", addedUser.Email);
            Assert.Equal("password1", addedUser.Password);

            // Verify user is added to the database
            var dbUser = await _context.Users.FindAsync(addedUser.Id);
            Assert.NotNull(dbUser);
            Assert.Equal("user1@test.com", dbUser.Email);
        }

        // Test for GetById method
        [Fact]
        public async Task GetById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", Password = "password1", FirstName = "a", LastName = "b" };
            await _repository.AddUser(user); // Add user to DB

            // Act
            var result = await _repository.GetById(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
        }

        // Test for GetById method when user does not exist
        [Fact]
        public async Task GetById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Act
            var result = await _repository.GetById(999); // ID that does not exist

            // Assert
            Assert.Null(result);
        }

        // Test for FindUser method (successful login)
        [Fact]
        public async Task FindUser_ShouldReturnUser_WhenCredentialsMatch()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", Password = "password1", FirstName = "a", LastName = "b" };
            await _repository.AddUser(user); // Add user to DB

            var loginUser = new ExisitingUser { Email = "user1@test.com", Password = "password1" };

            // Act
            var result = await _repository.login(loginUser.Email, loginUser.Password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user1@test.com", result.Email);
            Assert.Equal("password1", result.Password);
        }

        // Test for FindUser method (unsuccessful login)
        [Fact]
        public async Task FindUser_ShouldReturnNull_WhenCredentialsDoNotMatch()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", Password = "password1", FirstName="a", LastName="b" };
            await _repository.AddUser(user); // Add user to DB

            var loginUser = new ExisitingUser { Email = "user1@test.com", Password = "wrongpassword" };

            // Act
            var result = await _repository.login(loginUser.Email, loginUser.Password);

            // Assert
            Assert.Null(result);
        }

        // Test for UpdateUser method
        [Fact]
        public async Task UpdateUser_ShouldUpdateUser()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", Password = "password1", FirstName = "a", LastName = "b" };
            await _repository.AddUser(user); // Add user to DB

            user.Password = "newpassword"; // Change password

            // Act
            await _repository.UpdateUserAsync(user);

            // Assert
            var updatedUser = await _repository.GetById(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("newpassword", updatedUser.Password);
        }

    }
}