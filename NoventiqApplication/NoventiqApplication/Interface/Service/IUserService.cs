namespace NoventiqApplication.Interface
{
    public partial interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task AddUserAsync(UserDTO userDTO);
        Task UpdateUserAsync(UserDTO userDTO);
        Task DeleteUserAsync(int id);
        bool VerifyPassword(string hashedPassword, string plainPassword);
    }
}
