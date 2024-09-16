using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(string connectionString, ILogger<BookRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public Book GetBookById(int id)
        {
            try
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
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"-------->Error in GetBookById: {ex.Message}");
                throw; // Throw the exception to return a 500 Internal Server Error in the API
            }

            return null;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            var books = new List<Book>();

            try
            {
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
                                IsLoaned = IsBookLoaned(reader.GetInt32(reader.GetOrdinal("BookID")))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"-------->Error in GetAllBooks: {ex.Message}");
                throw;
            }

            return books;
        }

        public IEnumerable<Book> GetBooksByCategory(int categoryId)
        {
            var books = new List<Book>();

            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"-------->Error in GetBooksByCategory: {ex.Message}");
                throw;
            }

            return books;
        }

        public IEnumerable<OverdueBook> GetOverdueBooks()
        {
            var overdueBooks = new List<OverdueBook>();

            try
            {
                _logger.LogInformation("Connecting to database...");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    _logger.LogInformation("Database connection established.");
                    var command = new MySqlCommand(@"SELECT b.BookID, b.Name AS BookName, b.Author, m.Name AS MemberName, m.Surname, l.DateOfLoan, l.EndDate 
FROM books b 
JOIN loans l ON b.BookID = l.BookID 
JOIN members m ON l.MemberID = m.MemberID
WHERE l.Status = 'Active' 
AND l.EndDate <= CURDATE() - INTERVAL 1 DAY;", connection);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var loanStartDate = reader.GetDateTime(reader.GetOrdinal("DateOfLoan"));
                            var endDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                            var daysOverdue = (DateTime.Now - endDate).Days;

                            overdueBooks.Add(new OverdueBook
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("BookID")),
                                Title = reader.GetString(reader.GetOrdinal("BookName")),
                                Author = reader.GetString(reader.GetOrdinal("Author")),
                                BorrowerName = reader.GetString(reader.GetOrdinal("MemberName")) + " " +
                                               reader.GetString(reader.GetOrdinal("Surname")),
                                LoanStartDate = loanStartDate,
                                DaysOverdue = daysOverdue > 0 ? daysOverdue : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"-------->Error in GetOverdueBooks: {ex.Message}");
                throw;
            }

            return overdueBooks;
        }

        public bool IsBookLoaned(int bookId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogInformation($"-------->Error in IsBookLoaned: {ex.Message}");
                throw;
            }
        }
    }
}
