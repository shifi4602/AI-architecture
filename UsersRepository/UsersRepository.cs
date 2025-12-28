using Enteties;
using Microsoft.EntityFrameworkCore;
using Repositories;
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

        public async Task<User> login(string email, string password)
        {
            return await _apiShopContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task UpdateUserAsync(User userToUpdate)
        {
            _apiShopContext.Users.Update(userToUpdate);
            await _apiShopContext.SaveChangesAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _apiShopContext.Users.FindAsync(id);
        }

    }
}


