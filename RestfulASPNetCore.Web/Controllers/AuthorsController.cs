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
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public AuthorsController(ILibraryRepository repo, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            _repo = repo;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = nameof(GetAuthors))]
        public IActionResult GetAuthors([FromQuery]AuthorsResourceParameters parameters)
        {

            if (!_propertyMappingService.ValidMappingExistsFor<Dtos.Author, Entities.Author>(parameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<Dtos.Author>(parameters.Fields))
            {
                return BadRequest();
            }

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

            return Ok(result.ShapeData(parameters.Fields));
        }


        private string CreateAuthorsResourceUri(
        AuthorsResourceParameters parameters, ResourceUriType type)
        {
            return _urlHelper.Link(nameof(GetAuthors), AuthorsPagingData.GeneratePage(type, parameters));
        }

        [HttpGet("{id}", Name = nameof(GetAuthor))]
        public IActionResult GetAuthor(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<Dtos.Author>(fields))
            {
                return BadRequest();
            }

            var author = _repo.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<Dtos.Author>(author);
            return Ok(result.ShapeData(fields));
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