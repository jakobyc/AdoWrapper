using AdoWrapperCore.Data.Attributes;
using AdoWrapperCore.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdoWrapperCore.Data.Maps
{
    public class ColumnMap
    {
        public T MapSingle<T>(IDataReader reader) where T : new()
        {
            T mapType = new T();

            if (reader.Read())
            {
                foreach (PropertyInfo property in mapType.GetType().GetProperties())
                {
                    ColumnAttribute attribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                    if (attribute != null)
                    {
                        property.SetValue(mapType, reader[attribute.Name].ToString());
                    }
                }
            }
            return mapType;
        }

        public ICollection<T> MapCollection<T>(IDataReader reader) where T : new()
        {
            ICollection<T> collection = new List<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            IEnumerable<string> columns = reader.GetColumns();

            if (columns != null && columns.Count() > 0)
            {
                while (reader.Read())
                {
                    T mapType = new T();

                    foreach (PropertyInfo property in properties)
                    {
                        ColumnAttribute attribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                        // If attribute exists and a column exists with the attribute's value:
                        if (attribute != null && columns.Any(x => x == attribute.Name))
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(attribute.Name)))
                            {
                                property.SetValue(mapType, reader[attribute.Name]);
                            }
                        }
                    }

                    collection.Add(mapType);
                }
            }
            return collection;
        }
    }
}
