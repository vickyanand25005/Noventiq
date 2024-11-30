namespace NoventiqApplication
{
    public class UserDTO
    {
        public int? Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<int> RoleIds { get; set; }
        public List<RoleDTO>? Roles { get; set; }
    }

}
