using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace RestfulASPNetCore.Web.Dtos
{
    public class Pagination
    {
        public Pagination()
        {
            IncludeLinks = false;
        }

        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }

        public bool ShouldNextPageLink()
        {
            return IncludeLinks;
        }
        public bool ShouldSerializePreviousPageLink()
        {
            return IncludeLinks;
        }


        [JsonIgnore]
        public bool IncludeLinks { get; set; }

        internal static void AddHeader(HttpResponse response, Pagination pagination)
        {
            response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(pagination));
        }
    }
}
