using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Services;
using System;
using System.Collections.Generic;

namespace RestfulASPNetCore.Web.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private ILibraryRepository _libraryRepository;
        private ILogger<BooksController> _logger;

        public BooksController(ILibraryRepository repository, ILogger<BooksController> logger)
        {
            _libraryRepository = repository;
            _logger = logger;
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

        [HttpGet("{id}/author/{authorid}", Name = nameof(GetBookForAuthor))]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var book = _libraryRepository.GetBookForAuthor(authorId, id);
            if (book == null)
            {
                return NotFound();
            }

            var results = Mapper.Map<Book>(book);
            return Ok(results);
        }

        [HttpPost("author/{authorid}")]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] CreateBook book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (book.Description == book.Title)
            {
                ModelState.AddModelError(nameof(CreateBook), "The description should differ from the title.");
            }
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var newBook = Mapper.Map<Entities.Book>(book);
            _libraryRepository.AddBookForAuthor(authorId, newBook);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Couldn't save new book for Author {authorId}");
            }

            var bookToReturn = Mapper.Map<Book>(newBook);

            return CreatedAtRoute(nameof(GetBookForAuthor), new { authorId, id = newBook.Id }, bookToReturn);

        }

        [HttpDelete("{id}/author/{authorid}", Name = nameof(DeleteBookForAuthor))]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            var book = _libraryRepository.GetBookForAuthor(authorId, id);
            if (book == null)
            {
                return NotFound();
            }
            _libraryRepository.DeleteBook(book);
            if (!_libraryRepository.Save())
            {
                throw new Exception($"Failed to Delete book {id} for author {authorId}.");
            }

            _logger.LogInformation(100, $"Deleted book {id} for Author {authorId}");

            return NoContent();

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

        [HttpPut("{id}/author/{authorid}", Name = nameof(UpdateBookForAuthor))]
        public IActionResult UpdateBookForAuthor(Guid id, Guid authorId,
        [FromBody] UpdateBook book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (book.Description == book.Title)
            {
                ModelState.AddModelError(nameof(UpdateBook), "The description should differ from the title.");
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }


            var existingBook = _libraryRepository.GetBookForAuthor(authorId, id);
            if (existingBook == null)
            {
                var bookToAdd = Mapper.Map<Entities.Book>(book);
                bookToAdd.Id = id;
                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Upserting couldn't save the book {id}");
                }
                var bookToReturn = Mapper.Map<Book>(bookToAdd);

                return CreatedAtRoute(nameof(GetBookForAuthor), new { authorId, id }, bookToReturn);
            }


            Mapper.Map(book, existingBook);
            _libraryRepository.UpdateBookForAuthor(existingBook);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Cound't save the updated book {id}");
            }

            return NoContent();
        }

        [HttpPatch("{id}/author/{authorId}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid id, Guid authorId, [FromBody] JsonPatchDocument<UpdateBook> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var existingBook = _libraryRepository.GetBookForAuthor(authorId, id);
            if (existingBook == null)
            {
                var bookDto = new UpdateBook();
                patchDocument.ApplyTo(bookDto, ModelState);


                TryValidateModel(bookDto);

                if (ModelState.IsValid && bookDto.Description == bookDto.Title)
                {
                    ModelState.AddModelError(nameof(CreateBook), "The description should differ from the title.");
                }

                if (!ModelState.IsValid)
                {
                    return UnprocessableEntity(ModelState);
                }
                var bookToAdd = Mapper.Map<Entities.Book>(bookDto);
                bookToAdd.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Upserting couldn't save the book {id}");
                }
                var bookToReturn = Mapper.Map<Book>(bookToAdd);

                return CreatedAtRoute(nameof(GetBookForAuthor), new { authorId, id }, bookToReturn);
            }

            var bookToPatch = Mapper.Map<UpdateBook>(existingBook);

            patchDocument.ApplyTo(bookToPatch, ModelState);

            TryValidateModel(bookToPatch);
            
            if (ModelState.IsValid && bookToPatch.Description == bookToPatch.Title)
            {
                ModelState.AddModelError(nameof(CreateBook), "The description should differ from the title.");
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            Mapper.Map(bookToPatch, existingBook);

            _libraryRepository.UpdateBookForAuthor(existingBook);
            if (!_libraryRepository.Save())
            {
                throw new Exception($"Patching book {id} for author {authorId} failed to save");
            }

            return NoContent();
        }

    }
}