using AdoWrapperCore.Data.Attributes;
using AdoWrapperCore.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
                IDictionary<PropertyInfo, string> mappableProperties = GetMappableProperties(typeof(T).GetProperties(), columns);
                if (mappableProperties != null && mappableProperties.Count > 0)
                {
                    if (reader.Read())
                    {
                        MapProperties(mappableProperties, mapType, reader);
                    }
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
                /*IDictionary<PropertyInfo, string> mappableProperties = GetMappableProperties(properties, columns);
                if (mappableProperties != null && mappableProperties.Count > 0)
                {
                    while (reader.Read())
                    {
                        T mapType = MapProperties<T>(mappableProperties, reader);
                        if (mapType != null)
                        {
                            collection.Add(mapType);
                        }
                    }
                }*/
            }

            return collection;
        }

        /// <summary>
        /// Return a dictionary with mappable properties as the key and the respective column name as the value.
        /// </summary>
        /// <param name="columns">Valid column names.</param>
        private IDictionary<PropertyInfo, string> GetMappableProperties(PropertyInfo[] properties, IEnumerable<string> columns)
        {
            // Property/ColumnName pairs:
            IDictionary<PropertyInfo, string> mappableProperties = new Dictionary<PropertyInfo, string>();

            foreach (PropertyInfo property in properties)
            {
                ColumnAttribute attribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                if (attribute != null && columns.Any(x => x == attribute.Name))
                {
                    mappableProperties.Add(property, attribute.Name);
                }
            }
            return mappableProperties;
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

        private void MapProperties(IDictionary<PropertyInfo, string> mappableProperties, object mappedObject, IDataReader reader)
        {
            foreach (KeyValuePair<PropertyInfo, string> propertyColumnPair in mappableProperties)
            {
                MapColumn(propertyColumnPair, mappedObject, reader);
            }

            /*foreach (PropertyInfo property in properties)
            {

                MapColumnAttribute(property, columns, mappedObject, reader);

                MapHasColumnsAttribute(property, columns, mappedObject, reader);
            }*/
        }
    }
}
