using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace AdoWrapper.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// Create a connection via the connection string in your config file.
        /// </summary>
        /// <param name="name">Name of the connection string in your config file.</param>
        public IDbConnection CreateConnection(string name)
        {
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[name];

            return CreateConnection(connectionString);
        }

        public IDbConnection CreateConnection(string name, string provider)
        {
            if (!string.IsNullOrEmpty(name))
            {
                ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[name];

                if (!string.IsNullOrEmpty(provider))
                {
                    DbProviderFactory providerFactory = DbProviderFactories.GetFactory(provider);
                    IDbConnection connection = providerFactory.CreateConnection();
                    connection.ConnectionString = connectionString.ConnectionString;
                    connection.Open();

                    return connection;
                }
                else
                {
                    DbProviderFactory providerFactory = DbProviderFactories.GetFactory(connectionString.ProviderName);
                    IDbConnection connection = providerFactory.CreateConnection();
                    connection.ConnectionString = connectionString.ConnectionString;
                    connection.Open();

                    return connection;
                }
            }
            else
            {
                throw new Exception("Please provide the name of your connection string from your config file.");
            }
        }

        public IDbConnection CreateConnection(ConnectionStringSettings connectionString)
        {
            return CreateConnection(connectionString.ConnectionString, connectionString.ProviderName);
        }


        /*private void Validate(params string[] arguments)
        {
            foreach (string argument in arguments)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    throw new Exception(string.Format("Connection string is not valid."));
                }
            }
        }*/
    }
}
