using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulASPNetCore.Web.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new PropertyMappingValue(new List<string>() {  "Id" }) },
            { "Genre", new PropertyMappingValue(new List<string>() {  "Genre" }) },
            { "Age", new PropertyMappingValue(new List<string>() {  "DateOfBirth" }, true) },
            { "Name", new PropertyMappingValue(new List<string>() {  "FirstName", "LastName" }) }
           };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<Dtos.Author, Entities.Author>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }
            throw new Exception($"Cannot find exact property mapping instrance for <{typeof(TSource)}>");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(fields)) return true;

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit.Reverse())
            {
                var fieldSplit = field.Split(" ");

                if (!propertyMapping.ContainsKey(fieldSplit[0]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
