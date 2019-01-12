using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AdoWrapper.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// Create a connection via a connection string.
        /// </summary>
        /// <param name="name">Name of the connection string in your config file.</param>
        public IDbConnection CreateConnection<T>(string connectionString) where T: IDbConnection, new()
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                IDbConnection connection = new T();
                connection.ConnectionString = connectionString;
                connection.Open();

                return connection;
            }
            else
            {
                throw new Exception("Invalid connection string.");
            }
        }
    }
}
