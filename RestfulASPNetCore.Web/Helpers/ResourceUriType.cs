using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Http.Headers;

namespace RestfulASPNetCore.Web.Helpers
{
    public enum ResourceUriType
    {
        //Normally this would be None =0 , but given we are dealing with navigation this works
        Current = 0,
        Previous = 1,
        Next = 2,
    }

    public class VendorMediaType
    {
        public static string HateoasMediaType = "application/vnd.marvin.hateos+json";
        public static MediaType HateoasLinks = new MediaType(HateoasMediaType);
    }

}
