using AdoWrapper.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdoWrapper.Data.Maps
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

            while(reader.Read())
            {
                T mapType = new T();

                foreach (PropertyInfo property in properties)
                {
                    ColumnAttribute attribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                    if (attribute != null)
                    {
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(attribute.Name)))
                            {
                                property.SetValue(mapType, reader[attribute.Name]);
                            }
                        }
                        catch (IndexOutOfRangeException) { throw new Exception($"Column, '{attribute.Name}' not found."); }

                    }
                }

                collection.Add(mapType);
            }

            return collection;
        }
    }
}
