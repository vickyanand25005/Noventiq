namespace NoventiqApplication.Interface
{
    public interface IDapperRepository
    {
        Task<int> ExecuteAsync(string sql, object parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null);
    }
}
