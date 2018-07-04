using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Services;
using System;
using System.Collections.Generic;

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

            return Ok();
        }
    }
}