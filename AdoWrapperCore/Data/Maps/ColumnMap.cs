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
        internal T MapSingle<T>(IDataReader reader) where T : new()
        {
            T mapType = new T();
            IEnumerable<string> columns = reader.GetColumns();

            if (columns != null && columns.Count() > 0)
            {
                if (reader.Read())
                {
                    mapType = MapProperties<T>(mapType.GetType().GetProperties(), columns, reader);
                }
            }
            
            return mapType;
        }

        internal ICollection<T> MapCollection<T>(IDataReader reader) where T : new()
        {
            ICollection<T> collection = new List<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            IEnumerable<string> columns = reader.GetColumns();

            if (columns != null && columns.Count() > 0)
            {
                while (reader.Read())
                {
                    T mapType = MapProperties<T>(properties, columns, reader);
                    if (mapType != null)
                    {
                        collection.Add(mapType);
                    }
                }
            }
            return collection;
        }

        /// <summary>
        /// Check to see if the property contains a ColumnAttribute. If it does, attempt to get a value from the data reader and map it to 
        /// the mapped object.
        /// </summary>
        /// <param name="mappedObject">Original mapped object.</param>
        private void MapColumnAttribute(PropertyInfo property, IEnumerable<string> columns, object mappedObject, IDataReader reader)
        {
            ColumnAttribute attribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            // If attribute exists and a column exists with the attribute's value:
            if (attribute != null && columns.Any(x => x == attribute.Name))
            {
                if (!reader.IsDBNull(reader.GetOrdinal(attribute.Name)))
                {
                    property.SetValue(mappedObject, reader[attribute.Name]);
                }
            }
        }

        /// <summary>
        /// Check to see if the property has a HasColumns attribute. If it does, map all nested ColumnAttributes/HasColumnsAttributes
        /// and update the original mapped object.
        /// </summary>
        /// <param name="mappedObject">Original mapped object.</param>
        private void MapHasColumnsAttribute(PropertyInfo property, IEnumerable<string> columns, object mappedObject, IDataReader reader)
        {
            HasColumnsAttribute attrib = property.GetCustomAttribute(typeof(HasColumnsAttribute)) as HasColumnsAttribute;
            if (attrib != null)
            {
                object childProperty = Activator.CreateInstance(property.PropertyType);
                PropertyInfo[] innerProperties = property.PropertyType.GetProperties();

                foreach (PropertyInfo innerProperty in innerProperties)
                {
                    // Recursively handle nested HasColumns attributes:
                    MapHasColumnsAttribute(innerProperty, columns, childProperty, reader);

                    MapColumnAttribute(innerProperty, columns, childProperty, reader);
                }
                property.SetValue(mappedObject, childProperty);
            }
        }

        private T MapProperties<T>(PropertyInfo[] properties, IEnumerable<string> columns, IDataReader reader) where T : new()
        {
            T mappedObject = new T();
            foreach (PropertyInfo property in properties)
            {
                MapColumnAttribute(property, columns, mappedObject, reader);

                MapHasColumnsAttribute(property, columns, mappedObject, reader);
            }

            return mappedObject;
        }
    }
}
