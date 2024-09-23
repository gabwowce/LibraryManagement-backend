using LibraryManagement.Models;

namespace LibraryManagement.Interfaces
{
    public interface IBookRepository
    {
        Book GetBookById(int id);
        OverdueBook GetOverdueBookById(int loanId);
        IEnumerable<Book> GetAllBooks();
        IEnumerable<Book> GetBooksByCategory(int categoryId);
        IEnumerable<OverdueBook> GetOverdueBooks();
        bool IsBookLoaned(int bookId);
        bool EditOverdueBook(int loanId, string newEndDate = null, string markAsReturned = null);

        bool UploadNewBook(BookDto newBook);
        bool EditBook(int bookId, BookDto updatedBook);
        bool DeleteBookById(int bookId);
        bool HasActiveLoans(int bookId);



    }
}
