using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BooksController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet("{id}")]
    public ActionResult<Book> GetBookById(int id)
    {
        var book = _bookRepository.GetBookById(id);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }

    [HttpGet("category/{categoryId}")]
    public ActionResult<IEnumerable<Book>> GetBooksByCategory(int categoryId)
    {
        var books = _bookRepository.GetBooksByCategory(categoryId);
        return Ok(books);
    }

    [HttpGet("overdue")]
    public ActionResult<IEnumerable<OverdueBook>> GetOverdueBooks()
    {
        var overdueBooks = _bookRepository.GetOverdueBooks();
        return Ok(overdueBooks);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAllBooks()
    {
        var books = _bookRepository.GetAllBooks();
        return Ok(books);
    }
}
