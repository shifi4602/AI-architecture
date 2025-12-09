using Enteties;
using Repositories;

namespace Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _iUsersRepository;
        private readonly IpasswordServices _passwordServices;

        public UsersService(IUsersRepository usersRepository, IpasswordServices passwordServices)
        {
            _iUsersRepository = usersRepository;
            _passwordServices = passwordServices;
        }
        public async Task<User> AddNewUser(User user)
        {
            if (_passwordServices.GetStrength(user.Password).Strength <= 2)
                return null;
            return await _iUsersRepository.AddUser(user);
        }

        public async Task<User> Login(UpdateUser user)
        {
            return await _iUsersRepository.login(user);
        }

        public async Task<bool> UpdateUser(int id, User userToUpdate)
        {
            if (_passwordServices.GetStrength(userToUpdate.Password).Strength <= 2)
            {
                return false;
            }
            await _iUsersRepository.UpdateUserAsync(id, userToUpdate);
            return true;
        }


    }
}
