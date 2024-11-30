using AutoMapper;
using NoventiqApplication.Interface;

namespace NoventiqApplication.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public async Task<RoleDTO> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role == null ? null : _mapper.Map<RoleDTO>(role);
        }

        public async Task<bool> IsRoleNameUniqueAsync(string name)
        {
            return !await _roleRepository.ExistsByNameAsync(name);
        }

        public async Task<bool> IsRoleDescriptionUniqueAsync(string description)
        {
            return !await _roleRepository.ExistsByDescriptionAsync(description);
        }

        public async Task AddRoleAsync(RoleDTO roleDTO)
        {
            if (!await IsRoleNameUniqueAsync(roleDTO.Name))
                throw new ArgumentException("Role name must be unique.");

            var role = _mapper.Map<Role>(roleDTO);
            await _roleRepository.AddAsync(role);

            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateRoleAsync(RoleDTO roleDTO)
        {
            var existingRole = await _roleRepository.GetByIdAsync(roleDTO.Id.GetValueOrDefault());
            if (existingRole == null)
                throw new ArgumentException("Role does not exist.");

            if (!await IsRoleNameUniqueAsync(roleDTO.Name) && existingRole.Name != roleDTO.Name)
                throw new ArgumentException("Role name must be unique.");

            if (!await IsRoleDescriptionUniqueAsync(roleDTO.Description) && existingRole.Description != roleDTO.Description)
                throw new ArgumentException("Role description must be unique.");

            var role = _mapper.Map<Role>(roleDTO);
            _roleRepository.UpdateAsync(role);

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new ArgumentException("Role does not exist.");

            _roleRepository.DeleteAsync(role);

            await _unitOfWork.CommitAsync();
        }
    }

}
