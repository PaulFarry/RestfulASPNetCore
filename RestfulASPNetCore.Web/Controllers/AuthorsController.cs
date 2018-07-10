using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Helpers;
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
        private IUrlHelper _urlHelper;

        public AuthorsController(ILibraryRepository repo, IUrlHelper urlHelper)
        {
            _repo = repo;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = nameof(GetAuthors))]
        public IActionResult GetAuthors([FromQuery]AuthorsResourceParameters parameters)
        {
            var authors = _repo.GetAuthors(parameters);

            var prev = authors.HasPrevious ? CreateAuthorsResourceUri(parameters, ResourceUriType.Previous) : null;
            var next = authors.HasNext ? CreateAuthorsResourceUri(parameters, ResourceUriType.Next) : null;

            var pagination = new Pagination()
            {
                NextPageLink = next,
                PreviousPageLink = prev,
                TotalCount = authors.TotalCount,
                PageSize = authors.PageSize,
                CurrentPage = authors.CurrentPage,
                TotalPages = authors.TotalPages,
            };

            Pagination.AddHeader(Response, pagination);


            var result = Mapper.Map<IEnumerable<Dtos.Author>>(authors);

            return Ok(result);
        }

        private string CreateAuthorsResourceUri(
        AuthorsResourceParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.Previous:
                    return _urlHelper.Link(nameof(GetAuthors),
                        new
                        {
                            pageNumber = parameters.PageNumber - 1,
                            pageSize = parameters.PageSize
                        });
                case ResourceUriType.Next:
                    return _urlHelper.Link(nameof(GetAuthors),
                        new
                        {
                            pageNumber = parameters.PageNumber + 1,
                            pageSize = parameters.PageSize
                        });
                default:
                    return _urlHelper.Link(nameof(GetAuthors),
                        new
                        {
                            pageNumber = parameters.PageNumber,
                            pageSize = parameters.PageSize
                        });
            }
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


        [HttpPost("{id}")]

        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_repo.AuthorExists(id))
            {
                return Conflict();
            }
            return NotFound();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var author = _repo.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }
            _repo.DeleteAuthor(author);
            if (!_repo.Save())
            {
                throw new Exception($"Failed to delete author {id}");
            }

            return NoContent();



        }
    }
}