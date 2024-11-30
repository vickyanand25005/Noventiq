using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace NoventiqApplication.Repositories
{
    public class DapperRepository
    {
        private readonly string _connectionString;

        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new SqliteConnection(_connectionString);

        public async Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            using var dbConnection = Connection;
            return await dbConnection.ExecuteAsync(sql, parameters);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object parameters = null)
        {
            using var dbConnection = Connection;
            return await dbConnection.QuerySingleOrDefaultAsync<T>(sql, parameters);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
        {
            using var dbConnection = Connection;
            return await dbConnection.QueryAsync<T>(sql, parameters);
        }
    }
}