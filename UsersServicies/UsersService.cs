
using Enteties;
using Repositories;

namespace Services
{
    public class UsersService : IUsersService
    {
        IUsersRepository _iUsersRepository;
        IpasswordServices _iPasswordServices;
        public UsersService(IUsersRepository usersRepository, IpasswordServices passwordServices) 
        {
            _iUsersRepository = usersRepository;
            _iPasswordServices = passwordServices;
        }
        public async Task<User> AddNewUser(User user)
        {
            if (_iPasswordServices.GetStrength(user.Password).Strength <= 2)
                return null;
            return await _iUsersRepository.AddUser(user);
        }

        public  async Task<User> Login(UpdateUser user)
        {
            return await _iUsersRepository.login(user);
        }

        public async Task<bool> UpdateUser(int id, User userToUpdate)
        {
            if (_iPasswordServices.GetStrength(userToUpdate.Password).Strength <= 2)
            {
                return false;
            }
            _iUsersRepository.UpdateUserAsync(id, userToUpdate);
            return true;

        }
    }
}
