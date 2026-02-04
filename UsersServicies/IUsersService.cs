using DTO_s;
using Enteties;

namespace Services
{
    public interface IUsersService
    {
        Task<UserDTO> AddNewUser(UserDTO userDTO, string password);
        Task<User> Login(ExisitingUser user);
        Task<bool> UpdateUser(int id, UserDTO userToUpdate, string password);
        Task<UserDTO> GetById(int id);
    }
}