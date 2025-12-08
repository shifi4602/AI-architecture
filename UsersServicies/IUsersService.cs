using Enteties;

namespace Services
{
    public interface IUsersService
    {
        Task<User> AddNewUser(User user);
        Task<User> Login(UpdateUser user);
        Task<bool> UpdateUser(int id, User userToUpdate);
    }
}