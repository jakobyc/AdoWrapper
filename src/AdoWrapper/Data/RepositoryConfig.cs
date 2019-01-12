using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AdoWrapper.Data
{
    public class RepositoryConfig
    {
        public string ConnectionString { get; private set; }

        public IDbConnection Connection { get; private set; }

        public RepositoryConfig(string connectionString, Type connection)
        {
            ChangeConnection(connectionString, connection);
        }

        public void ChangeConnection(string connectionString, Type connection = null)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;

                if (connection != null && typeof(IDbConnection).IsAssignableFrom(connection))
                {
                    Connection = Activator.CreateInstance(connection) as IDbConnection;
                }
                else
                {
                    // If a connection type wasn't provided/valid, and Connection is null, throw an exception: 
                    if (Connection == null)
                    {
                        throw new Exception("Please provide a IDbConnection type (i.e., SqlConnection, SqliteConnection).");
                    }
                }
            }
            else
            {
                throw new Exception("Invalid connection string.");
            }
        }
    }
}
