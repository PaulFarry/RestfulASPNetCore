using Microsoft.AspNetCore.Mvc;
using RestfulASPNetCore.Web.Helpers;
using RestfulASPNetCore.Web.Models;
using RestfulASPNetCore.Web.Services;
using System;
using System.Collections.Generic;

namespace RestfulASPNetCore.Web.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {

        private ILibraryRepository _repo;

        public AuthorsController(ILibraryRepository repo)
        {
            _repo = repo;
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authors = _repo.GetAuthors();
            var result = new List<AuthorDto>();
            foreach (var author in authors)
            {
                var a = new AuthorDto
                {
                    Genre = author.Genre,
                    Name = $"{author.FirstName} {author.LastName}",
                    Id = author.Id,
                    Age = author.DateOfBirth.GetCurrentAge()
                };
                result.Add(a);
            }
            return new JsonResult(result);
        }
    }
}