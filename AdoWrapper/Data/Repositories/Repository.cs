using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace AdoWrapper.Data.Repositories
{
    public abstract class Repository : IRepository
    {
        private IDbConnection connection;

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
            connection = ConnectionFactory.CreateConnection(name);
        }

        public Repository(string name, string provider)
        {
            connection = ConnectionFactory.CreateConnection(name, provider);
        }

        public Repository(string name, IConnectionFactory connectionFactory)
        {
            connection = ConnectionFactory.CreateConnection(name);
        }

        public Repository(string name, string provider, IConnectionFactory connectionFactory)
        {
            connection = ConnectionFactory.CreateConnection(name, provider);
        }

        /// <summary>
        /// Create a command by string.
        /// </summary>
        public virtual IDbCommand CreateCommand(string query)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = query;
            
            return command;
        }

        public IDbTransaction BeginTransaction()
        {
            return connection.BeginTransaction();
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
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }
        }

        protected virtual void AddParameter(IDbCommand command, string parameter, object value)
        {
            IDbDataParameter param = command.CreateParameter();
            param.ParameterName = parameter;
            param.Value = value;

            command.Parameters.Add(param);
        }

        protected DataTable GetDataTable(IDbCommand command)
        {
            DataTable dt = new DataTable();

            using (IDataReader reader = command.ExecuteReader())
            {
                dt.Load(reader);    
            }

            return dt;
        }
    }
}
