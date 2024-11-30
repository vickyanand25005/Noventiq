namespace NoventiqApplication.Services
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByDescriptionAsync(string description);
    }
}