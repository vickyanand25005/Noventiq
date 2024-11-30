namespace NoventiqApplication.Interface
{
    public interface IPasswordService
    {
        string HashPassword(string plainPassword);
        bool VerifyPassword(string hashedPassword, string plainPassword);
    }
}
