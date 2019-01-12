using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoWrapper.Data.Attributes
{
    public class ColumnAttribute : Attribute
    {
        public string Name;

        public ColumnAttribute(string name)
        {
            this.Name = name;
        }
    }
}
