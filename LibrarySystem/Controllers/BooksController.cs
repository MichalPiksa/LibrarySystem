using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    static private List<Book> books = new List<Book>
    {
        new Book
        {
            Id = Guid.NewGuid(),
            Title = "The Chronicles of Narnia",
            Author = "C. S. Lewis",
            PublishedYear = 1950
        },
        new Book
        {
            Id = Guid.NewGuid(),
            Title = "The Hobbit",
            Author = "J. R. R. Tolkien",
            PublishedYear = 1937
        }
    };

    [HttpGet]
    public ActionResult<List<Book>> GetBooks()
    {
        return Ok(books);
    }

    [HttpGet("{id}")]
    public ActionResult<Book> GetBookById(Guid id)
    {
        var book = books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [HttpPost]
    public IActionResult CreateBook([FromBody] Book newBook)
    {
        if (newBook == null)
        {
            return BadRequest();
        }

        newBook.Id = Guid.NewGuid();
        books.Add(newBook);
        return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateBook(Guid id, [FromBody] Book updatedBook)
    {
        if (updatedBook == null)
        {
            return BadRequest();
        }

        var existingBook = books.FirstOrDefault(b => b.Id == id);
        if (existingBook == null)
        {
            return NotFound();
        }

        existingBook.Title = updatedBook.Title;
        existingBook.Author = updatedBook.Author;
        existingBook.PublishedYear = updatedBook.PublishedYear;
        return Ok(existingBook);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteBook(Guid id)
    {
        var book = books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }
        
        books.Remove(book);
        return NoContent();
    }
}