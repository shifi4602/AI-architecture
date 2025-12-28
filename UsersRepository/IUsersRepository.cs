using Enteties;
using Repositories;

namespace Repositories
{
    public interface  IUsersRepository
    {
        Task<User> AddUser(User user);
        Task<User> login(string email, string password);
        Task UpdateUserAsync(User userToUpdate);
        Task<User> GetById(int id);
    }
}