using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Helpers;
using System.Collections.Generic;

namespace RestfulASPNetCore.Web.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }


        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot([FromHeader(Name = HeaderNames.Accept)] string mediaType)
        {
            var currentMediaType = new MediaType(mediaType);
            var includeLinks = currentMediaType.IsSubsetOf(VendorMediaType.HateoasLinks);
            if (includeLinks)
            {
                var links = new List<Link>
                {
                    new Link(_urlHelper.Link(nameof(GetRoot), new { }), "self", "GET"),
                    new Link(_urlHelper.Link(nameof(AuthorsController.GetAuthors), new { }), "authors", "GET"),
                    new Link(_urlHelper.Link(nameof(AuthorsController.CreateAuthor), new { }), "create_author", "POST")
                };
                return Ok(links);
            }
            return NoContent();
        }

    }
}