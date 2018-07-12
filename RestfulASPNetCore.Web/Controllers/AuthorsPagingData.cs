using RestfulASPNetCore.Web.Helpers;

namespace RestfulASPNetCore.Web.Controllers
{
    public class AuthorsPagingData
    {
        public static AuthorsPagingData GeneratePage(ResourceUriType uriType, AuthorsResourceParameters parameters)
        {
            var pageData = new AuthorsPagingData
            {
                orderBy = parameters.OrderBy,
                searchQuery = parameters.SearchQuery,
                genre = parameters.Genre,
                pageSize = parameters.PageSize,
                pageNumber = parameters.PageNumber,
                fields = parameters.Fields,
            };

            switch (uriType)
            {
                case ResourceUriType.Previous:
                    pageData.pageNumber = parameters.PageNumber - 1;
                    break;
                case ResourceUriType.Next:
                    pageData.pageNumber = parameters.PageNumber + 1;
                    break;
            }
            return pageData;


        }

        public string searchQuery { get; set; }
        public string genre { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string orderBy { get; set; }
        public string fields { get; set; }
    }
}
