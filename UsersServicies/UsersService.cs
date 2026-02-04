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
        public async Task<UserDTO> AddNewUser(UserDTO userDTO, string password)
        {
            User user = _mapper.Map<UserDTO, User>(userDTO);
            user.Password = password;
            User userResult = await _iUsersRepository.AddUser(user);
            UserDTO userDTOres = _mapper.Map<User, UserDTO>(userResult);
            if (_passwordServices.GetStrength(user.Password).Strength <= 2)
                return null;
            return userDTOres;
        }

        public async Task<User> Login(ExisitingUser user)
        {
            return await _iUsersRepository.login(user.Email, user.Password);
        }

        public async Task<bool> UpdateUser(int id, UserDTO userToUpdate, string password)
        {
            if (_passwordServices.GetStrength(password).Strength <= 2)
            {
                return false;
            }
            User user = _mapper.Map<UserDTO, User>(userToUpdate);
            user.Id = id;
            user.Password = password;
            await _iUsersRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<UserDTO> GetById(int id)
        {
            User user = await _iUsersRepository.GetById(id);
            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }
    }
}
