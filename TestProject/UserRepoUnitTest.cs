using Enteties;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositories;
using System.Threading.Tasks;
using Xunit;

namespace Repositories.Tests
{
    public class UsersRepositoryTests
    {
        private readonly ApiShopContext _context;
        private readonly IUsersRepository _repository;

        public UsersRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApiShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique in-memory DB for each test
                .Options;

            _context = new ApiShopContext(options); // Use _context as the class variable
            _repository = new UsersRepository(_context); // Use _repository as the class variable
        }

        [Fact]
        public async Task AddUser_ReturnsUser_WhenUserIsAdded()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", FirstName="a", LastName="b",  Password = "password1" };

            // Act
            var result = await _repository.AddUser(user);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id); // Ensure the ID is set after adding
            Assert.Equal("user1@test.com", result.Email);
            Assert.Equal("password1", result.Password);
        }

        [Fact]
        public async Task Login_ReturnsUser_WhenCredentialsMatch()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", FirstName = "a", LastName = "b", Password = "password1" };
            await _repository.AddUser(user);

            var loginUser = new ExisitingUser { Email = "user1@test.com", Password = "password1" };

            // Act
            var result = await _repository.login(loginUser.Email, loginUser.Password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user1@test.com", result.Email);
            Assert.Equal("password1", result.Password);
        }

        [Fact]
        public async Task Login_ReturnsNull_WhenCredentialsDoNotMatch()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", FirstName = "a", LastName = "b", Password = "password1" };
            await _repository.AddUser(user);

            var loginUser = new ExisitingUser { Email = "user1@test.com", Password = "wrongpassword" };

            // Act
            var result = await _repository.login(loginUser.Email, loginUser.Password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Login_ReturnsUser_WhenValidCredentials()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", FirstName = "a", LastName = "b", Password = "password1" };
            await _repository.AddUser(user); // Ensure the user is added

            // Act
            var result = await _repository.login("user1@test.com", "password1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user1@test.com", result.Email);
            Assert.Equal("password1", result.Password);
        }

        [Fact]
        public async Task UpdateUser_UpdatesUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", FirstName = "a", LastName = "b", Password = "password1" };
            await _repository.AddUser(user);

            user.Password = "newpassword";

            // Act
            await _repository.UpdateUserAsync(user);
            var updatedUser = await _repository.GetById(1);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("newpassword", updatedUser.Password);
        }

        [Fact]
        public async Task GetById_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Email = "user1@test.com", FirstName = "a", LastName = "b", Password = "password1" };
            await _repository.AddUser(user); // Ensure the user is added

            // Act
            var result = await _repository.GetById(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Password, result.Password);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999; // A non-existing ID

            // Act
            var result = await _repository.GetById(userId);

            // Assert
            Assert.Null(result);
        }


    }
}
