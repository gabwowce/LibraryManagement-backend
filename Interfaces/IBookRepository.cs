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
    }
}
