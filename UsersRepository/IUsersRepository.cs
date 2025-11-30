using Enteties;

namespace Repositories
{
    public interface IUsersRepository
    {
        Users AddUser(Users value);
        Users login(UpdateUser value);
        void UpdateUser(int id, Users userToUpdate);
    }
}