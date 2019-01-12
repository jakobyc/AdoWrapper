using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace AdoWrapperCore.Data
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

        private static DbProviderFactory GetDbProviderFactory(string provider)
        {
            if (provider == "System.Data.SqlClient")
            {
                return SqlClientFactory.Instance;
            }
            else
            {
                Assembly assembly = Assembly.Load(provider);
                if (assembly != null)
                {
                    FieldInfo instance = assembly.GetType(GetProviderFactoryName(provider)).GetField("Instance");
                    DbProviderFactory factory = instance.GetValue(null) as DbProviderFactory;

                    return factory;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the full name of a provider factory.
        /// </summary>
        private static string GetProviderFactoryName(string provider)
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
