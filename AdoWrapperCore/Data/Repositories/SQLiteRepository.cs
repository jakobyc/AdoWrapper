using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoWrapperCore.Data.Repositories
{
    /// <summary>
    /// SQLite repository.
    /// </summary>
    public class SQLiteRepository : Repository
    {
        private const string provider = "Microsoft.Data.Sqlite";

        public SQLiteRepository(string connectionString) : base(provider, connectionString)
        {
        }
    }
}
