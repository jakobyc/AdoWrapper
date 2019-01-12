using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AdoWrapper.Data
{
    internal class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// Create a connection.
        /// </summary>
        /// <param name="name">Name of the connection string in your config file.</param>
        public IDbConnection CreateConnection(RepositoryConfig config)
        {
            if (config != null)
            {
                IDbConnection connection = config.Connection;
                connection.ConnectionString = config.ConnectionString;
                connection.Open();

                return connection;
            }
            return null;
        }
    }
}
