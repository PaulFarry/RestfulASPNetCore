using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Helpers;
using RestfulASPNetCore.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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

            if (!_propertyMappingService.ValidMappingExistsFor<Author, Entities.Author>(parameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<Author>(parameters.Fields))
            {
                return BadRequest();
            }

            var authors = _repo.GetAuthors(parameters);

            var pagination = new Pagination()
            {
                TotalCount = authors.TotalCount,
                PageSize = authors.PageSize,
                CurrentPage = authors.CurrentPage,
                TotalPages = authors.TotalPages,
            };

            Pagination.AddHeader(Response, pagination);

            var result = Mapper.Map<IEnumerable<Author>>(authors);

            var links = CreateLinks(parameters, authors.HasNext, authors.HasPrevious);

            var shapedAuthors = result.ShapeData(parameters.Fields);

            var shapedAuthorsWithLinks = shapedAuthors.Select(a =>
            {
                var authorDictionary = a as IDictionary<string, object>;
                var authorLinks = CreateLinks((Guid)authorDictionary[nameof(Author.Id)], parameters.Fields);
                authorDictionary.Add("links", authorLinks);
                return authorDictionary;
            }
            );

            var linkCollection = new { value = shapedAuthorsWithLinks, links };

            return Ok(linkCollection);
        }


        private string CreateAuthorsResourceUri(
        AuthorsResourceParameters parameters, ResourceUriType type)
        {
            return _urlHelper.Link(nameof(GetAuthors), AuthorsPagingData.GeneratePage(type, parameters));
        }

        private IEnumerable<Link> CreateLinks(Guid id, string fields)
        {
            var links = new List<Link>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new Link(_urlHelper.Link(nameof(GetAuthor), new { id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new Link(_urlHelper.Link(nameof(GetAuthor), new { id, fields }),
                "self",
                "GET"));
            }

            links.Add(new Link(_urlHelper.Link(nameof(GetAuthor), new { id }),
            "delete_author",
            "DELETE"));

            links.Add(new Link(_urlHelper.Link(nameof(BooksController.CreateBookForAuthor), new { authorId = id }),
            "create_book_for_author",
            "POST"));

            links.Add(new Link(_urlHelper.Link(nameof(BooksController.GetBooksForAuthor), new { authorId = id }),
            "books",
            "POST"));

            return links;
        }

        public IEnumerable<Link> CreateLinks(AuthorsResourceParameters authorsResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<Link>();

            links.Add(new Link(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(new Link(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Next), "nextPage", "GET"));
            }

            if (hasNext)
            {
                links.Add(new Link(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Previous), "previousPage", "GET"));
            }


            return links;
        }

        [HttpGet("{id}", Name = nameof(GetAuthor))]
        public IActionResult GetAuthor(Guid id, [FromQuery] string fields)
        {
            if (!_typeHelperService.TypeHasProperties<Author>(fields))
            {
                return BadRequest();
            }

            var author = _repo.GetAuthor(id);
            if (author == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<Author>(author);

            var links = CreateLinks(id, fields);
            var linkedResourceToReturn = result.ShapeData(fields) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return Ok(linkedResourceToReturn);
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
            var createdAuthor = Mapper.Map<Author>(newAuthor);

            var links = CreateLinks(createdAuthor.Id, null);

            var linkedResourceToReturn = createdAuthor.ShapeData(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);


            return CreatedAtRoute(nameof(GetAuthor), new { id = linkedResourceToReturn[nameof(Author.Id)] }, linkedResourceToReturn);
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

        [HttpDelete("{id}", Name = nameof(DeleteAuthor))]
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