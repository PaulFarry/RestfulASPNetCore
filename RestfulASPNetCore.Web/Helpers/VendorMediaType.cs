using Microsoft.AspNetCore.Mvc.Formatters;

namespace RestfulASPNetCore.Web.Helpers
{
    public class VendorMediaType
    {
        public const string HateoasLinks = "application/vnd.marvin.hateos+json";
        public static MediaType HateoasLinksMediaType = new MediaType(HateoasLinks);

        public const string NewAuthor = "application/vnd.marvin.author.full+json";
        public static MediaType NewAuthorMediaType = new MediaType(NewAuthor);

        public const string NewAuthorDead = "application/vnd.marvin.authordead.full+json";
        public static MediaType NewAuthorDeadMediaType = new MediaType(NewAuthorDead);
    }
}
