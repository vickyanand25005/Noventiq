namespace NoventiqApplication.Interface
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
