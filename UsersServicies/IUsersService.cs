using Enteties;

namespace Services
{
    public interface IUsersService
    {
        Users AddNewUser(Users user);
        Users Login(UpdateUser user);
        bool UpdateUser(int id, Users userToUpdate);
    }
}