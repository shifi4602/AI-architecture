
using Enteties;
using Repositories;

namespace Services
{
    public class UsersService : IUsersService
    {

        private readonly UsersRepository _usersRepository = new UsersRepository();
        private readonly passwordServices _passwordServices = new passwordServices();
        
        public Users AddNewUser(Users user)
        {
            if (_passwordServices.GetStrength(user.Password).Strength < 2)
                return null;
            return _usersRepository.AddUser(user);
        IUsersRepository _iUsersRepository;
        IpasswordServices _iPasswordServices;
        public UsersService(IUsersRepository usersRepository, IpasswordServices passwordServices) 
        {
            _iUsersRepository = usersRepository;
            _iPasswordServices = passwordServices;
        }
        public Users AddNewUser(Users user)
        {
            if (_iPasswordServices.GetStrength(user.Password).Strength <= 2)
                return null;
            return _iUsersRepository.AddUser(user);
        }

        public Users Login(UpdateUser user)
        
            return _usersRepository.Login(user);

            return _iUsersRepository.login(user);

        }

        public bool UpdateUser(int id, Users userToUpdate)
        {
            _usersRepository.UpdateUser(id, userToUpdate);
            if (_iPasswordServices.GetStrength(userToUpdate.Password).Strength <= 2)
            {
                return false;
            }
            _iUsersRepository.UpdateUser(id, userToUpdate);
            return true;
        }
    }
}
