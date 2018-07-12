using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RestfulASPNetCore.Web.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {

            if (source == null) throw new ArgumentNullException(nameof(source));

            //create a list to hold the objects.
            var expandoList = new List<ExpandoObject>();

            var propertyInfoList = new List<PropertyInfo>();

            var props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var keyedProperties = props.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(fields))
            {
                propertyInfoList.AddRange(keyedProperties.Values);
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');
                foreach (var field in fieldsAfterSplit)
                {
                    var propName = field.Trim();
                    if (keyedProperties.TryGetValue(propName, out var propertyInfo))
                    {
                        propertyInfoList.Add(propertyInfo);
                    }
                }
            }
            foreach (var sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();
                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    dataShapedObject.TryAdd(propertyInfo.Name, propertyValue);
                }
                expandoList.Add(dataShapedObject);
            }
            return expandoList;
        }
    }
}
