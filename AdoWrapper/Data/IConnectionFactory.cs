using System.Configuration;
using System.Data;

namespace AdoWrapper.Data
{
    public interface IConnectionFactory
    {
        /// <summary>
        /// Create a connection with a connection string.
        /// </summary>
        IDbConnection CreateConnection(ConnectionStringSettings connectionString);

        /// <summary>
        /// Create a connection with a connection string name.
        /// </summary>
        /// <param name="name">Connection string name.</param>
        IDbConnection CreateConnection(string name);

        /// <summary>
        /// Create a connection with a connection string name and a provider name.
        /// </summary>
        /// <param name="name">Connection string name.</param>
        /// <param name="provider">ADO.NET provider name.</param>
        IDbConnection CreateConnection(string name, string provider);
    }
}