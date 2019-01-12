using System.Configuration;
using System.Data;

namespace AdoWrapper.Data
{
    internal interface IConnectionFactory
    {
        /// <summary>
        /// Create and open a connection.
        /// </summary>
        /// <param name="name">Connection string name.</param>
        IDbConnection CreateConnection(RepositoryConfig config);
    }
}