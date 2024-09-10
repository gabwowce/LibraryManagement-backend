using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using System.Xml.Linq;

namespace LibraryManagement.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;

        public BookRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Book GetBookById(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Books WHERE BookID = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Book
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("BookID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Author = reader.GetString(reader.GetOrdinal("Author")),
                            YearOfRelease = reader.GetInt32(reader.GetOrdinal("YearOfRelease")),
                            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                            ImagePath = reader.GetString(reader.GetOrdinal("ImagePath"))
                        };
                    }
                }
            }
            return null;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            var books = new List<Book>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Books", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("BookID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Author = reader.GetString(reader.GetOrdinal("Author")),
                            YearOfRelease = reader.GetInt32(reader.GetOrdinal("YearOfRelease")),
                            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                            ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")),
                            IsLoaned = IsBookLoaned(reader.GetInt32(reader.GetOrdinal("BookID"))) // Patikriname, ar knyga yra paskolinta
                        });
                    }
                }
            }
            return books;
        }
        public bool IsBookLoaned(int bookId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(@"
                        SELECT COUNT(*) FROM Loans 
                        WHERE BookID = @BookId AND Status = 'Active'", connection);

            command.Parameters.AddWithValue("@BookId", bookId);

            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        public IEnumerable<Book> GetBooksByCategory(int categoryId)
        {
            var books = new List<Book>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Books WHERE CategoryID = @categoryId", connection);
                command.Parameters.AddWithValue("@categoryId", categoryId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("BookID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Author = reader.GetString(reader.GetOrdinal("Author")),
                            YearOfRelease = reader.GetInt32(reader.GetOrdinal("YearOfRelease")),
                            CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                            ImagePath = reader.GetString(reader.GetOrdinal("ImagePath"))
                        });
                    }
                }
            }
            return books;
        }
    }
}
