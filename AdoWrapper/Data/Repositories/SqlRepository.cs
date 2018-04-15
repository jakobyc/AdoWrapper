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
    }
}
