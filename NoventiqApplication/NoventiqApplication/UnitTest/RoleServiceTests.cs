using AutoMapper;
using Moq;
using NoventiqApplication.Interface;
using NoventiqApplication.Services;
using Xunit;

namespace NoventiqApplication.UnitTest
{
    public class RoleServiceTests
    {
        public IRoleService CreateRoleService(Mock<IRoleRepository> _mockRoleRepository, Mock<IMapper> _mockMapper, Mock<IUnitOfWork> _mockUnitOfWork)
        {
            _mockRoleRepository ??= new Mock<IRoleRepository>();
            _mockMapper ??= new Mock<IMapper>();
            _mockUnitOfWork ??= new Mock<IUnitOfWork>();
            return new RoleService(_mockRoleRepository.Object, _mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllRolesAsync_ShouldReturnAllRoles()
        {
            var _mockRoleRepository = new Mock<IRoleRepository>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();
            // Arrange
            var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin", Description = "Admin role" },
            new Role { Id = 2, Name = "User", Description = "User role" }
        };
            _mockRoleRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(roles);

            var roleDTOs = roles.Select(r => new RoleDTO { Id = r.Id, Name = r.Name, Description = r.Description }).ToList();
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<RoleDTO>>(roles)).Returns(roleDTOs);

            // Act
            var _roleService = CreateRoleService(_mockRoleRepository, _mockMapper, _mockUnitOfWork);
            var result = await _roleService.GetAllRolesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Name == "Admin");
            Assert.Contains(result, r => r.Name == "User");
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
        {
            var _mockRoleRepository = new Mock<IRoleRepository>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();
            // Arrange
            var role = new Role { Id = 1, Name = "Admin", Description = "Admin role" };
            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(role);
            _mockMapper.Setup(mapper => mapper.Map<RoleDTO>(role)).Returns(new RoleDTO { Id = role.Id, Name = role.Name, Description = role.Description });

            // Act
            var _roleService = CreateRoleService(_mockRoleRepository, _mockMapper, _mockUnitOfWork);
            var result = await _roleService.GetRoleByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Admin", result.Name);
        }

        [Fact]
        public async Task AddRoleAsync_ShouldCallRepositoryWithCorrectData()
        {
            var _mockRoleRepository = new Mock<IRoleRepository>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();
            // Arrange
            var roleDTO = new RoleDTO { Name = "Admin", Description = "Admin role" };
            var role = new Role { Name = "Admin", Description = "Admin role" };
            _mockMapper.Setup(mapper => mapper.Map<Role>(roleDTO)).Returns(role);

            // Act
            var _roleService = CreateRoleService(_mockRoleRepository, _mockMapper, _mockUnitOfWork);
            await _roleService.AddRoleAsync(roleDTO);

            // Assert
            _mockRoleRepository.Verify(repo => repo.AddAsync(It.Is<Role>(r => r.Name == "Admin" && r.Description == "Admin role")), Times.Once);
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldUpdateRepositoryWithCorrectData()
        {
            var _mockRoleRepository = new Mock<IRoleRepository>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();
            // Arrange
            var roleDTO = new RoleDTO { Id = 1, Name = "Updated Admin", Description = "Updated Admin role" };
            var role = new Role { Id = 1, Name = "Updated Admin", Description = "Updated Admin role" };
            Role capturedRole = null;

            _mockMapper.Setup(mapper => mapper.Map<Role>(roleDTO)).Returns(role);

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(role);
            _mockRoleRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Role>()))
                .Callback<Role>(r => capturedRole = r);

            var _roleService = CreateRoleService(_mockRoleRepository, _mockMapper, _mockUnitOfWork);
            // Act
            await _roleService.UpdateRoleAsync(roleDTO);

            // Assert
            Assert.NotNull(capturedRole);
            Assert.Equal(1, capturedRole.Id);
            Assert.Equal("Updated Admin", capturedRole.Name);
            Assert.Equal("Updated Admin role", capturedRole.Description);

        }


        [Fact]
        public async Task DeleteRoleAsync_ShouldDeleteRoleWithCorrectData()
        {
            var _mockRoleRepository = new Mock<IRoleRepository>();
            var _mockMapper = new Mock<IMapper>();
            var _mockUnitOfWork = new Mock<IUnitOfWork>();
            // Arrange
            var roleId = 1;
            var role = new Role { Id = roleId, Name = "Test Role", Description = "Test Description" };
            Role capturedRoleToDelete = null;

            _mockRoleRepository.Setup(repo => repo.GetByIdAsync(roleId)).ReturnsAsync(role);
            _mockRoleRepository
                .Setup(repo => repo.DeleteAsync(It.IsAny<Role>()))
                .Callback<Role>(r => capturedRoleToDelete = r);

            // Act
            var _roleService = CreateRoleService(_mockRoleRepository, _mockMapper, _mockUnitOfWork);
            await _roleService.DeleteRoleAsync(roleId);

            // Assert
            Assert.NotNull(capturedRoleToDelete);
            Assert.Equal(roleId, capturedRoleToDelete.Id);
            Assert.Equal("Test Role", capturedRoleToDelete.Name);
            Assert.Equal("Test Description", capturedRoleToDelete.Description);

        }


    }
}