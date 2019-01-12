using AdoWrapperCore.Data.Attributes;
using AdoWrapperCore.Data.Caching;
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
    internal class ColumnMap
    {
        private readonly AdoCache cache = new AdoCache();
        
        internal T MapSingle<T>(IDataReader reader) where T : new()
        {
            T mapType = new T();
            IEnumerable<string> columns = reader.GetColumns();

            if (columns != null && columns.Count() > 0)
            {
                if (reader.Read())
                {
                    mapType = MapColumns<T>(typeof(T).GetProperties(), columns, reader);
                }
            }
            
            return mapType;
        }

        internal ICollection<T> MapCollection<T>(IDataReader reader) where T : new()
        {
            ICollection<T> collection = new List<T>();
            IEnumerable<string> columns = reader.GetColumns();
            if (columns != null && columns.Count() > 0)
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                while (reader.Read())
                {
                    T mappedType = MapColumns<T>(properties, columns, reader);
                    if (mappedType != null)
                    {
                        collection.Add(mappedType);
                    }
                }
            }
            return collection;
        }

        /// <summary>
        /// Return a dictionary with mappable properties as the key and the respective column name as the value.
        /// </summary>
        /// <param name="columns">Valid column names.</param>
        private IDictionary<PropertyInfo, string> GetMappableProperties(PropertyInfo[] properties, IEnumerable<string> columns)
        {
            string key = $"mappableProps_{properties.FirstOrDefault()?.DeclaringType.AssemblyQualifiedName}";

            // Property/ColumnName pairs:
            IDictionary<PropertyInfo, string> mappableProperties = cache.GetOrAdd(key, () =>
            {
                IDictionary<PropertyInfo, string> mappableProps = new Dictionary<PropertyInfo, string>();

                foreach (PropertyInfo property in properties)
                {
                    ColumnAttribute attribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                    if (attribute != null)
                    {
                        mappableProps.Add(property, attribute.Name);
                    }
                }

                return mappableProps;
            });

            // Return properties that have valid column names:
            return mappableProperties.Where(x => columns.Any(c => c == x.Value)).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void MapColumn(KeyValuePair<PropertyInfo, string> propertyColumnPair, object mappedObject, IDataReader reader)
        {
            if (!reader.IsDBNull(reader.GetOrdinal(propertyColumnPair.Value)))
            {
                propertyColumnPair.Key.SetValue(mappedObject, reader[propertyColumnPair.Value]);
            }
        }

        private T MapColumns<T>(PropertyInfo[] properties, IEnumerable<string> columns, IDataReader reader, object mappedObject = null) where T : new()
        {
            mappedObject = mappedObject ?? new T();
            PropertyInfo[] propertiesWithColumns = properties.Where(x => x.IsDefined(typeof(HasColumnsAttribute))).ToArray();
            if (propertiesWithColumns != null && propertiesWithColumns.Length > 0)
            {
                foreach (PropertyInfo property in propertiesWithColumns)
                {
                    object childProp = Activator.CreateInstance(property.PropertyType);
                    
                    // Recursively check for nested columns:
                    MapColumns<T>(property.PropertyType.GetProperties(), columns, reader, childProp);

                    property.SetValue(mappedObject, childProp);
                }
            }

            // After checking for nested columns, start mapping:
            IDictionary<PropertyInfo, string> mappableProperties = GetMappableProperties(properties, columns);
            if (mappableProperties != null && mappableProperties.Count > 0)
            {
                MapProperties(mappableProperties, mappedObject, reader);
            }
            if (mappedObject is T)
            {
                return (T)mappedObject;
            }
            return default(T);
        }

        private void MapProperties(IDictionary<PropertyInfo, string> mappableProperties, object mappedObject, IDataReader reader)
        {
            foreach (KeyValuePair<PropertyInfo, string> propertyColumnPair in mappableProperties)
            {
                MapColumn(propertyColumnPair, mappedObject, reader);
            }
        }
    }
}
