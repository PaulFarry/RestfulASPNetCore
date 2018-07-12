using System.Collections.Generic;

namespace RestfulASPNetCore.Web.Services
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; private set; }

        public bool Reverse { get; private set; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool reverse = false)
        {
            DestinationProperties = destinationProperties;
            Reverse = reverse;
        }
    }
}
