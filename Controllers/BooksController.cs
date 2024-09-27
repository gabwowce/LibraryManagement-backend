using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using System.Collections.Generic;
using LibraryManagement.Repositories;

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
            return NotFound($"Book with ID {id} not found.");
        }
        return Ok(book);
    }

    [HttpGet("category/{categoryId}")]
    public ActionResult<IEnumerable<Book>> GetBooksByCategory(int categoryId)
    {
        var books = _bookRepository.GetBooksByCategory(categoryId);
        if (books == null || !books.Any())
        {
            return NotFound($"No books found for category ID {categoryId}.");
        }
        return Ok(books);
    }

    [HttpGet("overdue")]
    public ActionResult<IEnumerable<OverdueBook>> GetOverdueBooks()
    {
        var overdueBooks = _bookRepository.GetOverdueBooks();
        if (overdueBooks == null || !overdueBooks.Any())
        {
            return NotFound("No overdue books found.");
        }
        return Ok(overdueBooks);
    }

    [HttpGet("overdue/{loanId}")]
    public ActionResult<OverdueBook> GetOverdueBookById(int loanId)
    {
        var overdueBook = _bookRepository.GetOverdueBookById(loanId);
        if (overdueBook == null)
        {
            return NotFound($"Overdue book with loan ID {loanId} not found.");
        }
        return Ok(overdueBook);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Book>> GetAllBooks()
    {
        var books = _bookRepository.GetAllBooks();
        if (books == null || !books.Any())
        {
            return NotFound("No books found in the library.");
        }
        return Ok(books);
    }

    [HttpPut("overdue/{loanId}")]
    public IActionResult EditOverdueBook(int loanId, [FromBody] OverdueBookUpdateDto updateDto)
    {
        if (updateDto == null)
        {
            return BadRequest("Invalid data.");
        }

        var result = _bookRepository.EditOverdueBook(loanId, updateDto.newEndDate, updateDto.status);

        if (!result)
        {
            return NotFound($"Overdue book with loanId {loanId} not found.");
        }

        return Ok("Overdue book updated successfully.");
    }


    [HttpPost("book")]
    public IActionResult UploadNewBook([FromBody] BookDto newBook)
    {
        if (newBook == null)
        {
            return BadRequest("Invalid book data.");
        }

        var result = _bookRepository.UploadNewBook(newBook);

        if (!result)
        {
            return StatusCode(500, "An error occurred while creating the book.");
        }

        return Ok("Book created successfully.");
    }
    [HttpPut("book/{bookId}")]
    public IActionResult EditBook(int bookId, [FromBody] BookDto updatedBook)
    {
        if (updatedBook == null)
        {
            return BadRequest("Invalid book data.");
        }

        var result = _bookRepository.EditBook(bookId, updatedBook);

        if (!result)
        {
            return NotFound("Book not found.");
        }

        return NoContent(); 
    }

    [HttpDelete("{bookId}")]
    public IActionResult DeleteBook(int bookId)
    {
        var result = _bookRepository.DeleteBookById(bookId);

        if (!result)
        {
            return NotFound($"Book with ID {bookId} not found.");
        }

        return Ok($"Book with ID {bookId} deleted successfully.");
    }




    [HttpGet("check-loans/{bookId}")]
    public IActionResult CheckActiveLoans(int bookId)
    {
        var hasActiveLoans = _bookRepository.HasActiveLoans(bookId);
        return Ok(!hasActiveLoans); 
    }







}
