using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AdoWrapper.Data
{
    public static class DataReaderExtension
    {
        public static T Get<T>(this IDataReader reader, string column) where T : IComparable
        {
            try
            {
                int index = reader.GetOrdinal(column);

                if (!reader.IsDBNull(index))
                {
                    return (T)reader[index];
                }
            }
            catch (IndexOutOfRangeException) { throw new Exception($"Column, '{column}' not found."); }

            return default(T);
        }
    }
}
