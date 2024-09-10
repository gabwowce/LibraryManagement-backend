using LibraryManagement.Models;

namespace LibraryManagement.Interfaces
{
    public interface IBookRepository
    {
        Book GetBookById(int id);
        IEnumerable<Book> GetAllBooks();
        IEnumerable<Book> GetBooksByCategory(int categoryId);
    }
}
