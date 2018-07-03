﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Services;

namespace RestfulASPNetCore.Web.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private ILibraryRepository _libraryRepository;
        public BooksController(ILibraryRepository repository)
        {
            _libraryRepository = repository;
        }

        [HttpGet("author/{authorid}")]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var books = _libraryRepository.GetBooksForAuthor(authorId);


            var results = Mapper.Map<IEnumerable<Book>>(books);
            return Ok(results);
        }

        [HttpGet("{bookid}/author/{authorid}")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid bookId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var book = _libraryRepository.GetBookForAuthor(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }

            var results = Mapper.Map<Book>(book);
            return Ok(results);
        }

        [HttpGet("{bookid}")]
        public IActionResult GetBook(Guid bookId)
        {
            var book = _libraryRepository.GetBook(bookId);
            if (book == null)
            {
                return NotFound();
            }

            var results = Mapper.Map<Book>(book);
            return Ok(results);
        }

    }
}