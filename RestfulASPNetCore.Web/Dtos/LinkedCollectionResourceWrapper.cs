using System.Collections.Generic;

namespace RestfulASPNetCore.Web.Dtos
{
    public class LinkedCollectionResourceWrapper<T> : LinkedResourceBase
        where T : LinkedResourceBase
    {
        public IEnumerable<T> Value { get; set; }

        public LinkedCollectionResourceWrapper(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
