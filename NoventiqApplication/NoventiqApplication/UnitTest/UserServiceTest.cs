using AutoMapper;
using Moq;
using NoventiqApplication.Interface;
using NoventiqApplication.Services;
using Xunit;

namespace NoventiqApplication.UnitTest
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public IUserService CreateUserService(Mock<IUserRepository> _mockUserRepository, Mock<IPasswordService> _mockPasswordService, Mock<IMapper> _mockMapper,
            Mock<IUnitOfWork> _mockUnitOfWork)
        {
            _mockUserRepository ??= new Mock<IUserRepository>();
            _mockPasswordService ??= new Mock<IPasswordService>();
            _mockMapper ??= new Mock<IMapper>();
            _mockUnitOfWork ??= new Mock<IUnitOfWork>();
            return new UserService(_mockUserRepository.Object, _mockUnitOfWork.Object, _mockPasswordService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsersWithRoles()
        {
            // Arrange
            var _mockUserRepository = new Mock<IUserRepository>();
            var _mockPasswordService = new Mock<IPasswordService>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();
            var users = new List<User>
    {
        new User
        {
            Id = 1,
            Username = "john_doe",
            Email = "john@example.com",
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role { Id = 101, Name = "Admin", Description = "Administrator role" }
                }
            }
        },
    };

            _mockUserRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            var userDTOs = users.Select(user =>
            {
                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Roles = user.UserRoles.Select(ur => new RoleDTO
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        Description = ur.Role.Description
                    }).ToList()
                };
                return userDto;
            }).ToList();


            _mockMapper.Setup(mapper => mapper.Map<UserDTO>(It.Is<User>(u => u.Id == 1))).Returns(new UserDTO
            {
                Id = 1,
                Username = "john_doe",
                Email = "john@example.com",
                Roles = new List<RoleDTO>
        {
            new RoleDTO { Id = 101, Name = "Admin", Description = "Administrator role" }
        }
            });

            // Act
            var _userService = CreateUserService(_mockUserRepository, _mockPasswordService, _mockMapper, _mockUnitOfWork);
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());

            var user1 = result.First(u => u.Id == 1);
            Assert.Equal("john_doe", user1.Username);
            Assert.Equal("john@example.com", user1.Email);
            Assert.Single(user1.Roles);
            Assert.Equal(101, user1.Roles.First().Id);
            Assert.Equal("Admin", user1.Roles.First().Name);
        }


        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var _mockUserRepository = new Mock<IUserRepository>();
            var _mockPasswordService = new Mock<IPasswordService>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();

            var user = new User { Id = 1, Username = "anand_m", Email = "anand_m@example.com", UserRoles = new List<UserRole> { new UserRole { RoleId = 1, UserId = 1, Role = new Role { Id = 1, Name = "Test" } } } };
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<UserDTO>(user)).Returns(new UserDTO { Id = user.Id, Username = user.Username, Email = user.Email });

            // Act
            var _userService = CreateUserService(_mockUserRepository, _mockPasswordService, _mockMapper, _mockUnitOfWork);

            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("anand_m", result.Username);
        }

        [Fact]
        public async Task AddUserAsync_ShouldCallRepositoryWithCorrectData()
        {
            // Arrange
            var _mockUserRepository = new Mock<IUserRepository>();
            var _mockPasswordService = new Mock<IPasswordService>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();

            var userDTO = new UserDTO
            {
                Username = "anand_m",
                Email = "anand_m@example.com",
                PasswordHash = "plain_password"
            };

            var user = new User
            {
                Username = "anand_m",
                Email = "anand_m@example.com",
                PasswordHash = "hashed_password",
                UserRoles = new List<UserRole> { new UserRole { RoleId = 1, UserId = 1, Role = new Role { Id = 1, Name = "Test" } } }
            };

            User capturedUser = null;

            _mockMapper.Setup(mapper => mapper.Map<User>(userDTO)).Returns(user);
            _mockPasswordService.Setup(service => service.HashPassword("plain_password")).Returns("hashed_password");

            _mockUserRepository
                .Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u); // Capture the user passed to AddAsync

            var _userService = CreateUserService(_mockUserRepository, _mockPasswordService, _mockMapper, _mockUnitOfWork);

            // Act
            await _userService.AddUserAsync(userDTO);

            // Assert
            Assert.NotNull(capturedUser);
            Assert.Equal("anand_m", capturedUser.Username);
            Assert.Equal("anand_m@example.com", capturedUser.Email);
            Assert.Equal("hashed_password", capturedUser.PasswordHash);
            Assert.Single(capturedUser.UserRoles);
            Assert.Equal(1, capturedUser.UserRoles.First().RoleId);

        }


        [Fact]
        public async Task DeleteUserAsync_ShouldCallRepository_WhenUserExists()
        {
            var _mockUserRepository = new Mock<IUserRepository>();
            var _mockPasswordService = new Mock<IPasswordService>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Username = "anand_m", Email = "anand_m@example.com" };

            _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var _userService = CreateUserService(_mockUserRepository, _mockPasswordService, _mockMapper, _mockUnitOfWork);

            await _userService.DeleteUserAsync(userId);

            // Assert
            _mockUserRepository.Verify(repo => repo.DeleteAsync(It.Is<User>(u => u.Id == userId)), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);  // Ensure commit was called
        }
    }
}
