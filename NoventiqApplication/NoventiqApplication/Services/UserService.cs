using AutoMapper;
using NoventiqApplication.Interface;
using System.Text.RegularExpressions;

namespace NoventiqApplication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordService passwordService, IMapper mapper)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDTOs = users.Select(user =>
            {
                var userDto = _mapper.Map<UserDTO>(user);
                userDto.Roles = user.UserRoles.Select(ur => new RoleDTO
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Description = ur.Role.Description
                }).ToList();
                return userDto;
            }).ToList();

            return userDTOs;
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDTO>(user);
            userDto.Roles = user.UserRoles.Select(ur => new RoleDTO
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name,
                Description = ur.Role.Description
            }).ToList();

            return userDto;
        }

        public async Task AddUserAsync(UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.PasswordHash))
                throw new ArgumentException("Password cannot be empty.");

            if (!await IsUsernameUniqueAsync(userDTO.Username))
                throw new ArgumentException("Username must be unique.");

            if (!await IsEmailValidAsync(userDTO.Email))
                throw new ArgumentException("Invalid email format.");

            userDTO.PasswordHash = _passwordService.HashPassword(userDTO.PasswordHash);
            var usr = _mapper.Map<User>(userDTO);
            await _userRepository.AddAsync(usr);
            await _unitOfWork.CommitAsync();

            if (userDTO.RoleIds != null && userDTO.RoleIds.Any())
            {
                foreach (var roleId in userDTO.RoleIds)
                {
                    await _userRepository.AddUserRoleAsync(usr.Id, roleId);
                }
            }
            await _unitOfWork.CommitAsync();

        }

        public async Task UpdateUserAsync(UserDTO userDTO)
        {
            if (string.IsNullOrWhiteSpace(userDTO.PasswordHash))
                throw new ArgumentException("Password cannot be empty.");

            if (!await IsUsernameUniqueAsync(userDTO.Username) &&
                (await _userRepository.GetByIdAsync(userDTO.Id.GetValueOrDefault())).Username != userDTO.Username)
                throw new ArgumentException("Username must be unique.");

            if (!await IsEmailValidAsync(userDTO.Email))
                throw new ArgumentException("Invalid email format.");

            userDTO.PasswordHash = _passwordService.HashPassword(userDTO.PasswordHash);
            var usr = _mapper.Map<User>(userDTO);
            _userRepository.UpdateAsync(usr);

            await _userRepository.ClearUserRolesAsync(userDTO.Id.GetValueOrDefault());
            if (userDTO.RoleIds != null && userDTO.RoleIds.Any())
            {
                foreach (var roleId in userDTO.RoleIds)
                {
                    await _userRepository.AddUserRoleAsync(userDTO.Id.GetValueOrDefault(), roleId);
                }

            }
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.ClearUserRolesAsync(id);
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                _userRepository.DeleteAsync(user);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                throw new ArgumentException($"User with ID {id} does not exist.");
            }
        }


        public bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            return _passwordService.VerifyPassword(hashedPassword, plainPassword);
        }


        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _userRepository.ExistsByUsernameAsync(username);
        }

        public async Task<bool> IsEmailValidAsync(string email)
        {
            // Validate email format using Regex
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}
