using Enteties;
using Repositories.Models;

namespace Repositories
{
    public interface IUsersRepository
    {
        Task<User> AddUser(User user);
        Task<User> login(UpdateUser updateUser);
        Task UpdateUserAsync(int id, User userToUpdate);
    }
}