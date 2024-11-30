namespace NoventiqApplication.Interface
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        RefreshToken GenerateRefreshToken(int userId);
    }
}
