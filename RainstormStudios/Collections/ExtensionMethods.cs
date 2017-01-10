using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Collections
{
    public static class CollectionExtensions
    {
        static readonly System.Reflection.BindingFlags
            bindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.GetProperty;

        public static string AsXML<T>(this IEnumerable<T> src, string rootElement = "root")
        {
            // First, we have to determine the "Type" of the generic parameter.
            Type genType = typeof(T);

            // Get the properties for the generic Type "T".
            System.Reflection.PropertyInfo[] typeProps = genType.GetProperties(bindingFlags).Where(f => f.CanRead && !f.IsSpecialName && f.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0).ToArray();

            // Now, we loop through each item in the list and extract the property values.
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("<{0}>", rootElement));
            foreach (var listItem in src)
            {
                for (int t = 0; t < typeProps.Length; t++)
                {
                    object propVal = typeProps[t].GetValue(listItem, null);
                    string propValStr = (propVal != null ? propVal.ToString() : string.Empty);
                    sb.AppendLine(string.Format("<{0}>{1}</{0}>", typeProps[t].Name, propValStr));
                }
            }
            sb.AppendLine(string.Format("</{0}>", rootElement));
            return sb.ToString();
        }
    }
}
