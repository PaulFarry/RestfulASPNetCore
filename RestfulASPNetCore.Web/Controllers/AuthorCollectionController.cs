using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Helpers;
using RestfulASPNetCore.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulASPNetCore.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorCollectionController : ControllerBase
    {
        private ILibraryRepository repo;
        public AuthorCollectionController(ILibraryRepository libraryRepository)
        {
            repo = libraryRepository;
        }

        [HttpPost]
        public IActionResult Create([FromBody] IEnumerable<CreateAuthor> authors)
        {
            if (authors == null)
            {
                return BadRequest();
            }
            var authorEntities = Mapper.Map<IEnumerable<Entities.Author>>(authors);
            foreach (var author in authorEntities)
            {
                repo.AddAuthor(author);
            }

            if (!repo.Save())
            {
                throw new Exception("Creating authors failed to save");
            }

            var authorsCollectionToResult = Mapper.Map<IEnumerable<Author>>(authorEntities);
            var idsAsString = string.Join(",", authorsCollectionToResult.Select(x => x.Id));

            return CreatedAtRoute(nameof(GetAuthorCollection), new { ids = idsAsString }, authorsCollectionToResult);
        }
        [HttpGet("({ids})", Name = nameof(GetAuthorCollection))]
        public IActionResult GetAuthorCollection(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authors = repo.GetAuthors(ids);

            if (authors.Count() != ids.Count())
            {
                return NotFound();
            }
            var authorsToReturn = Mapper.Map<IEnumerable<Author>>(authors);

            return Ok(authorsToReturn);

        }
    }
}