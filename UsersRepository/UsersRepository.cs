using Enteties;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using System.Linq;
using System.Text.Json;

namespace Repositories
{
    public class UsersRepository : IUsersRepository
    {
        ApiShopContext _apiShopContext;
        public UsersRepository(ApiShopContext apiShopContext)
        {
            _apiShopContext = apiShopContext;
        }
        public async Task<User> AddUser(User user)
        {
            await _apiShopContext.Users.AddAsync(user);
            await _apiShopContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> login(UpdateUser updateUser)
        {
            return await _apiShopContext.Users.FirstOrDefaultAsync(x => x.Email == updateUser.Email && x.Password == updateUser.Password);
        }

        public async Task UpdateUserAsync(int id, User userToUpdate)
        {
            _apiShopContext.Users.Update(userToUpdate);
            await _apiShopContext.SaveChangesAsync();
        }
    }
}
