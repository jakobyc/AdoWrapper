using AdoWrapper.Data.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoWrapper.Data.Repositories
{
    /// <summary>
    ///  SQL Server repository.
    /// </summary>
    public class SqlRepository : Repository
    {
        private const string provider = "System.Data.SqlClient";

        public SqlRepository(string name) : base(name)
        {
        }

        /// <summary>
        /// Disable a job by job name.
        /// </summary>
        /// <param name="name">Name of a job on SQL Server.</param>
        /// <returns></returns>
        public virtual int DisableJob(string name)
        {
            AdoParameters parameters = new AdoParameters();
            parameters.Add("@name", name);

            return ExecuteNonQuery($@"
                            UPDATE msdb.dbo.sysjobs
                            SET enabled = 0
                            WHERE enabled = 1
                              AND name = @name", parameters);
        }

        /// <summary>
        /// Disable jobs by category name.
        /// </summary>
        /// <param name="category">Name of a job category on SQL Server.</param>
        public virtual int DisableJobs(string category)
        {
            AdoParameters parameters = new AdoParameters();
            parameters.Add("@category", category);

            return ExecuteNonQuery($@"
                            UPDATE msdb.dbo.sysjobs
                            SET enabled = 0
                            WHERE enabled = 1
                              AND category_id = (SELECT category_id FROM msdb.dbo.syscategories WHERE name = @category)", parameters);
        }

        /// <summary>
        /// Disable jobs by category id.
        /// </summary>
        /// <param name="categoryid">Id of a job category on SQL Server.</param>
        public virtual int DisableJobs(int categoryId)
        {
            AdoParameters parameters = new AdoParameters();
            parameters.Add("@category", categoryId);

            return ExecuteNonQuery($@"
                            UPDATE msdb.dbo.sysjobs
                            SET enabled = 0
                            WHERE enabled = 1
                              AND category_id = @category", parameters);
        }

        /// <summary>
        /// Enable a job by job name.
        /// </summary>
        /// <param name="name">Name of a job on SQL Server.</param>
        /// <returns></returns>
        public virtual int EnableJob(string name)
        {
            AdoParameters parameters = new AdoParameters();
            parameters.Add("@name", name);

            return ExecuteNonQuery($@"
                            UPDATE msdb.dbo.sysjobs
                            SET enabled = 1
                            WHERE enabled = 0
                              AND name = @name", parameters);
        }

        /// <summary>
        /// Enable jobs by category name.
        /// </summary>
        /// <param name="category">Name of a job category on SQL Server.</param>
        public virtual int EnableJobs(string category)
        {
            AdoParameters parameters = new AdoParameters();
            parameters.Add("@category", category);

            return ExecuteNonQuery($@"
                            UPDATE msdb.dbo.sysjobs
                            SET enabled = 1
                            WHERE enabled = 0
                              AND category_id = (SELECT category_id FROM msdb.dbo.syscategories WHERE name = @category)", parameters);
        }

        /// <summary>
        /// Enable jobs by category id.
        /// </summary>
        /// <param name="categoryid">Id of a job category on SQL Server.</param>
        public virtual int EnableJobs(int categoryId)
        {
            AdoParameters parameters = new AdoParameters();
            parameters.Add("@category", categoryId);

            return ExecuteNonQuery($@"
                            UPDATE msdb.dbo.sysjobs
                            SET enabled = 1
                            WHERE enabled = 0
                              AND category_id = @category", parameters);
        }
    }
}
