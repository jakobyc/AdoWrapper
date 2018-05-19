using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Reflection;

namespace AdoWrapperCore.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        /// <summary>
        /// Create a connection via a connection string.
        /// </summary>
        /// <param name="name">Name of the connection string in your config file.</param>
        public IDbConnection CreateConnection(string provider, string connectionString)
        {
            if (Valid(provider, connectionString))
            {
                DbProviderFactory providerFactory = GetDbProviderFactory(provider);
                if (providerFactory != null)
                {
                    IDbConnection connection = providerFactory.CreateConnection();
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    return connection;
                }
                else
                {
                    throw new Exception("Provider Factory not found.");
                }
            }
            return null;
        }

        public static DbProviderFactory GetDbProviderFactory(string provider)
        {
            Assembly assembly = Assembly.Load(provider);
            if (assembly != null)
            {
                //DbProviderFactory factory = a.CreateInstance($"{provider}.SqlFactory") as DbProviderFactory;
            }

            return null;
        }

        /// <summary>
        /// Get the full name of a provider factory.
        /// </summary>
        public static string GetProviderFactoryName(string provider)
        {
            switch (provider)
            {
                case ("Microsoft.Data.Sqlite"):
                    return "Microsoft.Data.Sqlite.SqliteFactory";

                case ("MySql.Data"):
                    return "MySql.Data.MySqlClient.MySqlClientFactory";

                case ("Npgsql"):
                    return "Npgsql.NpgsqlFactory";

                default:
                    return null;
            }
        }

        private bool Valid(string provider, string connectionString)
        {
            if (string.IsNullOrEmpty(provider))
            {
                throw new Exception("Please input a database provider.");
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Please provide a connection string.");
            }
            return true;
        }
    }
}
