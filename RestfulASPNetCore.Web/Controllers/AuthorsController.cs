using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

            var result = Mapper.Map<IEnumerable<Dtos.Author>>(authors);

            return new JsonResult(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = _repo.GetAuthor(id);
            var result = Mapper.Map<Dtos.Author>(author);
            return new JsonResult(result);
        }
    }
}