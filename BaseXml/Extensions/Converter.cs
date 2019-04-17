using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.XPath;

namespace BaseXml.Extensions
{
    public class Converter
    {
        public static object GetValue(object result)
        {
            if (result == null) { return null; }

            switch (result)
            {
                case bool value: return value.ToString();
                case double value: return double.IsNaN((double)result) ? 0 : value;
                case string value: return value;
                case XPathNodeIterator nodes:
                    var items = new List<string>();
                    while (nodes.MoveNext()) { items.Add(nodes.Current.Value); }

                    switch (items.Count)
                    {
                        case 0: return null;
                        default: return string.Join(" ", items);
                    }
                default: throw new NotImplementedException();
            }
        }

        public static IEnumerable<object> GetFromMultipleValues(object result)
        {
            if (result == null) { return null; }

            switch (result)
            {
                case XPathNodeIterator nodes:
                    var items = new List<string>();
                    while (nodes.MoveNext()) { items.Add(nodes.Current.Value); }

                    switch (items.Count)
                    {
                        case 0: return null;
                        default: return items;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        public static object ChangeType(object value, Type conversionType)
        {
            // check if nullable type was passed in
            var isNullable = false;
            Type safeType = Nullable.GetUnderlyingType(conversionType);
            if (safeType == null)
            {
                safeType = conversionType;
                isNullable = true;
            }

            switch (safeType.Name)
            {
                case "Boolean":
                    if (value == null || value.Equals("0")) { value = isNullable ? (bool?)null : false; }
                    else if (value.Equals("1")) { value = true; }
                    break;
                case "Int16":
                case "Int32":
                case "Int64":
                case "Double":
                case "Decimal":
                    if (value == null || value.Equals("NaN")
                        || (value is string && string.IsNullOrWhiteSpace((string)value)))
                    { if (isNullable) { value = null; } else { value = 0; } }
                    break;
            }

            if (value == null) { return null; }

            var converter = TypeDescriptor.GetConverter(safeType);
            if (converter.CanConvertFrom(value.GetType()))
            {
                if (value is string)
                { return converter.ConvertFromInvariantString((string)value); }
                else
                { return converter.ConvertFrom(value); }
            }
            else
            {
                return Convert.ChangeType(value, safeType, CultureInfo.InvariantCulture);
            }
        }
    }
}
