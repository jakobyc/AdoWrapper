using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using AdoWrapperCore.Data.Maps;
using AdoWrapperCore.Data.Parameters;

namespace AdoWrapperCore.Data.Repositories
{
    public abstract class Repository : IDisposable
    {
        /// <summary>
        /// Connection string.
        /// </summary>
        protected string ConnectionString { get; set; }
        /// <summary>
        /// Name of ADO.NET provider.
        /// </summary>
        protected string Provider { get; set; }

        private IConnectionFactory connectionFactory;
        protected IConnectionFactory ConnectionFactory
        {
            get
            {
                connectionFactory = connectionFactory ?? new ConnectionFactory();
                return connectionFactory;
            }
            set
            {
                connectionFactory = value;
            }
        }

        public Repository(string provider, string connectionString)
        {
            this.Provider = provider;
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Add a parameter to a command object.
        /// </summary>
        /// <param name="parameter">Parameter name (i.e., "@userId")</param>
        /// <param name="value">Value to replace the parameter name with.</param>
        protected virtual void AddParameter(IDbCommand command, string parameter, object value)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameter;
            param.Value = value;

            command.Parameters.Add(param);
        }

        /// <summary>
        /// Create a transaction.
        /// </summary>
        protected IDbTransaction BeginTransaction(IDbConnection connection)
        {
            return connection.BeginTransaction();
        }

        /// <summary>
        /// Create a connection to a database.
        /// </summary>
        protected virtual IDbConnection CreateConnection()
        {
            return ConnectionFactory.CreateConnection(Provider, ConnectionString);
        }

        /// <summary>
        /// Create a SQL command.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>Command object.</returns>
        protected virtual IDbCommand CreateCommand(IDbConnection connection, string query, IDictionary<string, object> parameters = null)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = query;

            if (parameters != null && parameters.Count > 0)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    AddParameter(command, parameter.Key, parameter.Value);
                }
            }
            return command;
        }

        /// <summary>
        /// Execute a SQL query and map the results to a collection of a type.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>Collection of type T.</returns>
        protected virtual ICollection<T> Execute<T>(string query, IDictionary<string, object> parameters = null) where T : new()
        {
            ICollection<T> collection = new List<T>();

            using (IDbConnection connection = CreateConnection())
            {
                using (IDbCommand command = CreateCommand(connection, query, parameters))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        ColumnMap map = new ColumnMap();

                        collection = map.MapCollection<T>(reader);
                    }
                }
            }
            return collection;
        }

        /// <summary>
        /// Execute a SQL command and return the DataReader.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>DataReader</param>
        protected virtual IDataReader ExecuteReader(IDbConnection connection, string query, IDictionary<string, object> parameters = null)
        {
            using (IDbCommand command = CreateCommand(connection, query, parameters))
            {
                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Execute a SQL command.
        /// </summary>
        /// <param name="sql">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>Affected row count.</returns>
        protected virtual int ExecuteNonQuery(string sql, IDictionary<string, object> parameters = null)
        {
            using (IDbConnection connection = CreateConnection())
            {
                using (IDbCommand command = CreateCommand(connection, sql, parameters))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Execute a stored procedure.
        /// </summary>
        protected virtual int ExecuteProcedure(string procedureName, IDictionary<string, object> parameters = null)
        {
            using (IDbConnection connection = CreateConnection())
            {
                using (IDbCommand command = CreateCommand(connection, procedureName, parameters))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    return command.ExecuteNonQuery();
                }
            }
        }

        protected virtual T ExecuteScalar<T>(string query, IDictionary<string, object> parameters = null) where T : IConvertible
        {
            using (IDbConnection connection = CreateConnection())
            {
                using (IDbCommand command = CreateCommand(connection, query, parameters))
                {
                    object o = command.ExecuteScalar();
                    if (o != null)
                    {
                        object casted = Convert.ChangeType(o, typeof(T));
                        return (T)casted;
                    }
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Execute a SQL command.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>DataTable.</returns>
        protected virtual DataTable GetDataTable(string query, IDictionary<string, object> parameters = null)
        {
            DataTable dt = new DataTable();

            using (IDbConnection connection = CreateConnection())
            {
                using (IDbCommand command = CreateCommand(connection, query, parameters))
                {
                    dt.Load(command.ExecuteReader());
                }
            }

            return dt;
        }

        /// <summary>
        /// Return a DataTable from a command.
        /// </summary>
        protected DataTable GetDataTable(IDbCommand command)
        {
            DataTable dt = new DataTable();
            
            using (IDataReader reader = command.ExecuteReader())
            {
                dt.Load(reader);
            }

            return dt;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (connectionFactory != null)
                {
                    connectionFactory = null;
                }

                if (!string.IsNullOrEmpty(Provider))
                {
                    Provider = null;
                }
            }
        }
    }
}
