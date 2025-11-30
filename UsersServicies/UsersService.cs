using Enteties;
using Repositories;

namespace Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository = new UsersRepository();
        private readonly IpasswordServices _passwordServices = new passwordServices();
        
        public UsersService(IUsersRepository usersRepository, IpasswordServices passwordServices)
        {
            _usersRepository = usersRepository;
            _passwordServices = passwordServices;
        }
        public Users AddNewUser(Users user)
        {
            if (_passwordServices.GetStrength(user.Password).Strength <= 2)
                return null;
            return _usersRepository.AddUser(user);
        }
        
        public Users Login(UpdateUser user)
        {
            return _usersRepository.Login(user);
        }
        
        public bool UpdateUser(int id, Users userToUpdate)
        {
            _usersRepository.UpdateUser(id, userToUpdate);
            if (_passwordServices.GetStrength(userToUpdate.Password).Strength <= 2)
            {
                return false;
            }
            _usersRepository.UpdateUser(id, userToUpdate);
            return true;
        }
    }
}
