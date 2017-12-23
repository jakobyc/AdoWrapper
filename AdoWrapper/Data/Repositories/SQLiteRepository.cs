using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoWrapper.Data.Repositories
{
    /// <summary>
    /// SQLite repository.
    /// </summary>
    public class SQLiteRepository : Repository
    {
        private const string provider = "System.Data.SQLite";

        public SQLiteRepository(string name) : base(name, provider)
        {
        }
    }
}
