using System.Collections.Generic;

namespace RestfulASPNetCore.Web.Dtos
{
    public abstract class LinkedResourceBase
    {
        public List<Link> Links { get; set; } = new List<Link>();

    }
}
