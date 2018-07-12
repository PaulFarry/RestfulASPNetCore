using RestfulASPNetCore.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;


namespace RestfulASPNetCore.Web.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy, Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (mappingDictionary == null) throw new ArgumentNullException(nameof(mappingDictionary));
            if (string.IsNullOrWhiteSpace(orderBy)) return source;

            var orderByAfterSplit = orderBy.Split(',');
            foreach (var orderByClause in orderByAfterSplit.Reverse())
            {
                var trimmedOrderBy = orderBy.Trim();
                var orderOperations = trimmedOrderBy.Split(" ");
                var orderDescending = false;
                if (orderOperations.Length > 1)
                {
                    orderDescending = orderOperations[1].StartsWith("d", StringComparison.OrdinalIgnoreCase);
                }

                var propertyName = orderOperations[0];


                if (mappingDictionary.TryGetValue(propertyName, out var propertyMappingValue))
                {
                    foreach (var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                    {
                        if (propertyMappingValue.Reverse)
                        {
                            orderDescending = !orderDescending;
                        }
                        source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                    }
                }
            }
            return source;
        }
    }
}
