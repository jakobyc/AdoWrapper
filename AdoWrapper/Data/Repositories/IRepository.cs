using System;
using System.Data;

namespace AdoWrapper.Data.Repositories
{
    public interface IRepository : IDisposable
    {
        IDbCommand CreateCommand(string query);
    }
}