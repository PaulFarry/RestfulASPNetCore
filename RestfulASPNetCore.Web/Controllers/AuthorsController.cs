using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestfulASPNetCore.Web.Dtos;
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

            return Ok(result);
        }

        [HttpGet("{id}", Name = nameof(GetAuthor))]
        public IActionResult GetAuthor(Guid id)
        {
            var author = _repo.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<Dtos.Author>(author);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody]CreateAuthor author)
        {
            if (author == null)
            {
                return BadRequest();
            }

            var newAuthor = Mapper.Map<Entities.Author>(author);
            _repo.AddAuthor(newAuthor);
            if (!_repo.Save())
            {
                throw new Exception("Failed to Create new author");
            }
            var createdAuthor = Mapper.Map<Dtos.Author>(newAuthor);
            return CreatedAtRoute(nameof(GetAuthor), new { id = newAuthor.Id }, createdAuthor);
        }
    }
}