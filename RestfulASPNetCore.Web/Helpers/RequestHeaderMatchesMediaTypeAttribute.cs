using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;

namespace RestfulASPNetCore.Web.Helpers
{
    public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
    {
        private readonly string _requestHeaderToMatch;
        private readonly string[] _mediaTypes;

        public RequestHeaderMatchesMediaTypeAttribute(string requestHeaderToMatch, string[] mediaTypes)
        {
            _requestHeaderToMatch = requestHeaderToMatch;
            _mediaTypes = mediaTypes;
        }

        public int Order => 0;

        //6:12 in video

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeaders = context.RouteContext.HttpContext.Request.Headers;

            if (requestHeaders.TryGetValue(_requestHeaderToMatch, out var matchedRequestHeader))
            {
                foreach (var mediaType in _mediaTypes)
                {
                    if (string.Equals(matchedRequestHeader, mediaType, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;

        }
    }
}
