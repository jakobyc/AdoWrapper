using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AdoWrapper.Data
{
    public abstract class RepositoryConfig
    {
        public string ConnectionString { get; protected set; }

        public IDbConnection Connection { get; protected set; }

        public void ChangeConnection<T>(string connectionString) where T : IDbConnection, new()
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;

                Connection = new T();
            }
            else
            {
                throw new Exception("Invalid connection string.");
            }
        }
    }

    public class RepositoryConfig<ConnectionType> : RepositoryConfig where ConnectionType : IDbConnection, new() 
    {
        public RepositoryConfig(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;

                Connection = new ConnectionType();
            }
            else
            {
                throw new Exception("Invalid connection string.");
            }
        }
    }
}
