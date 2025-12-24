using AutoMapper;
using DTO_s;
using Enteties;
using Repositories;

namespace Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _iUsersRepository;
        private readonly IpasswordServices _passwordServices;
        IMapper _mapper;

        public UsersService(IUsersRepository usersRepository, IpasswordServices passwordServices, IMapper imapper)
        {
            _iUsersRepository = usersRepository;
            _passwordServices = passwordServices;
            _mapper = imapper;
        }
        public async Task<UserDTO> AddNewUser(UserDTO userdto, string password)
        {
            User user = _mapper.Map<UserDTO, User>(userdto);
            user.Password = password;
            User userResult = await _iUsersRepository.AddUser(user);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(userResult);
            if (_passwordServices.GetStrength(user.Password).Strength <= 2)
                return null;
            return userDTO;
        }

        public async Task<User> Login(ExisitingUser user)
        {
            return await _iUsersRepository.login(user.Email, user.Password);
        }

        public async Task<bool> UpdateUser(int id, UserDTO userToUpdate, string password)
        {
            User user = _mapper.Map<UserDTO, User>(userToUpdate);
            user.Password = password;
            if (_passwordServices.GetStrength(user.Password).Strength <= 2)
            {
                return false;
            }
            await _iUsersRepository.UpdateUserAsync(id, user);
            return true;
        }


    }
}
