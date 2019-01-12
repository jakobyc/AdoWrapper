using System.Configuration;
using System.Data;

namespace AdoWrapper.Data
{
    public interface IConnectionFactory
    {
        /// <summary>
        /// Create a connection with a connection string.
        /// </summary>
        /// <param name="name">Connection string name.</param>
        IDbConnection CreateConnection<T>(string connectionString) where T : IDbConnection, new();
    }
}