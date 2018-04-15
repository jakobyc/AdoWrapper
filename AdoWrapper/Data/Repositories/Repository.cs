using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace AdoWrapper.Data.Repositories
{
    public abstract class Repository : IDisposable
    {
        /// <summary>
        /// Connection string name.
        /// </summary>
        protected string Name { get; set; }
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

        public Repository(string name)
        {
            this.Name = name;
        }

        public Repository(string name, string provider) : this(name)
        {
            this.Provider = provider;
        }

        public Repository(string name, IConnectionFactory connectionFactory) : this (name)
        {
            this.ConnectionFactory = connectionFactory;
        }

        public Repository(string name, string provider, IConnectionFactory connectionFactory) : this(name, provider)
        {
            this.ConnectionFactory = connectionFactory;
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
            return ConnectionFactory.CreateConnection(Name, Provider);
        }

        /// <summary>
        /// Create a command by string.
        /// </summary>
        protected virtual IDbCommand CreateCommand(IDbConnection connection, string query)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = query;
                
            return command;
        }

        /// <summary>
        /// Execute a SQL command.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>Command object.</returns>
        private IDbCommand Execute(IDbConnection connection, string query, IDictionary<string, object> parameters)
        {
            using (IDbCommand command = CreateCommand(connection, query))
            {
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        AddParameter(command, parameter.Key, parameter.Value);
                    }
                }
                return command;
            }
        }

        protected virtual IDataReader ExecuteReader(IDbConnection connection, string query)
        {
            return ExecuteReader(connection, query, null);
        }

        /// <summary>
        /// Execute a SQL command and return the DataReader.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>DataReader</param>
        protected virtual IDataReader ExecuteReader(IDbConnection connection, string query, IDictionary<string, object> parameters)
        {
            return Execute(connection, query, parameters).ExecuteReader();
        }

        protected virtual int ExecuteNonQuery(string query)
        {
            return ExecuteNonQuery(query, null);
        }

        /// <summary>
        /// Execute a SQL command.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>Affected row count.</returns>
        protected virtual int ExecuteNonQuery(string query, IDictionary<string, object> parameters)
        {
            using (IDbConnection connection = CreateConnection())
            {
                return Execute(connection, query, parameters).ExecuteNonQuery();
            }
        }

        protected virtual DataTable GetDataTable(string query)
        {
            return GetDataTable(query, null);
        }

        /// <summary>
        /// Execute a SQL command.
        /// </summary>
        /// <param name="query">SQL string.</param>
        /// <param name="parameters">Dictionary of parameters. Key = parameter name, value = parameter value.</param>
        /// <returns>DataTable.</returns>
        protected virtual DataTable GetDataTable(string query, IDictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();

            using (IDbConnection connection = CreateConnection())
            {
                dt.Load(Execute(connection, query, parameters).ExecuteReader());
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

                if (!string.IsNullOrEmpty(Name))
                {
                    Name = null;
                }

                if (!string.IsNullOrEmpty(Provider))
                {
                    Provider = null;
                }
            }
        }
    }
}
