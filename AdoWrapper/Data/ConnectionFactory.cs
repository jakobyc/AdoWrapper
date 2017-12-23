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
        public IDbConnection CreateConnection(string name)
        {
            Validate(name);

            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[name];

            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            IDbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString.ConnectionString;
            connection.Open();

            return connection;
        }

        public IDbConnection CreateConnection(string name, string provider)
        {
            Validate(name, provider);

            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(provider);
            IDbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            connection.Open();

            return connection;
        }

        public IDbConnection CreateConnection(ConnectionStringSettings connectionString)
        {
            Validate(connectionString.Name, connectionString.ProviderName);

            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(connectionString.ProviderName);
            IDbConnection connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString.ConnectionString;
            connection.Open();

            return connection;
        }

        private void Validate(params string[] arguments)
        {
            foreach (string argument in arguments)
            {
                if (string.IsNullOrEmpty(argument))
                {
                    throw new Exception(string.Format("Connection string is not valid."));
                }
            }
        }
    }
}
