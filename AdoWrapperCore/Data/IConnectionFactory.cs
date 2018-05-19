using System.Configuration;
using System.Data;

namespace AdoWrapperCore.Data
{
    public interface IConnectionFactory
    {
        /// <summary>
        /// Create a connection with a connection string.
        /// </summary>
        /// <param name="name">Connection string name.</param>
        IDbConnection CreateConnection(string provider, string connectionString);
    }
}