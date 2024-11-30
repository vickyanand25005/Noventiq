namespace NoventiqApplication.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(int id);
        Task<bool> IsRoleNameUniqueAsync(string name);
        Task<bool> IsRoleDescriptionUniqueAsync(string description);
        Task AddRoleAsync(RoleDTO roleDTO);
        Task UpdateRoleAsync(RoleDTO roleDTO);
        Task DeleteRoleAsync(int id);
    }

}
