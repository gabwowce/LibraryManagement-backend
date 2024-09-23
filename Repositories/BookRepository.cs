using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;

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
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand(@"
                        SELECT b.BookID, b.Name, b.Author, b.YearOfRelease, b.CategoryID, b.ImagePath, b.Quantity,
                               c.Category AS CategoryName,
                               (b.Quantity - COALESCE((SELECT COUNT(*) FROM Loans WHERE BookID = b.BookID AND Status = 'Active'), 0)) AS RemainingFree
                        FROM Books b
                        INNER JOIN Categories c ON b.CategoryID = c.CategoryID
                        WHERE b.BookID = @id", connection);
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
                                Category = reader.GetString(reader.GetOrdinal("CategoryName")), // Full category name
                                ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")),
                                Amount = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                RemainingFree = reader.GetInt32(reader.GetOrdinal("RemainingFree"))
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetBookById: {ex.Message}");
                throw;
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
                    var command = new MySqlCommand(@"
                        SELECT b.BookID, b.Name, b.Author, b.YearOfRelease, b.CategoryID, b.ImagePath, b.Quantity,
                               c.Category AS CategoryName,
                               (b.Quantity - COALESCE((SELECT COUNT(*) FROM Loans WHERE BookID = b.BookID AND Status = 'Active'), 0)) AS RemainingFree
                        FROM Books b
                        INNER JOIN Categories c ON b.CategoryID = c.CategoryID", connection);

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
                                Category = reader.GetString(reader.GetOrdinal("CategoryName")), 
                                ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")),
                                Amount = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                RemainingFree = reader.GetInt32(reader.GetOrdinal("RemainingFree"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetAllBooks: {ex.Message}");
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
                    var command = new MySqlCommand(@"
                        SELECT b.BookID, b.Name, b.Author, b.YearOfRelease, b.CategoryID, b.ImagePath, b.Quantity,
                               c.Category AS CategoryName,
                               (b.Quantity - COALESCE((SELECT COUNT(*) FROM Loans WHERE BookID = b.BookID AND Status = 'Active'), 0)) AS RemainingFree
                        FROM Books b
                        INNER JOIN Categories c ON b.CategoryID = c.CategoryID
                        WHERE b.CategoryID = @categoryId", connection);
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
                                Category = reader.GetString(reader.GetOrdinal("CategoryName")), 
                                ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")),
                                Amount = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                RemainingFree = reader.GetInt32(reader.GetOrdinal("RemainingFree"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetBooksByCategory: {ex.Message}");
                throw;
            }

            return books;
        }

        public IEnumerable<OverdueBook> GetOverdueBooks()
        {
            var overdueBooks = new List<OverdueBook>();

            try
            {
                Log.Error("Connecting to database...");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand(@"
                        SELECT b.BookID, b.Name AS BookName, b.Author, m.Name AS MemberName, m.Surname, l.DateOfLoan, l.EndDate, l.LoanID 
                        FROM Books b 
                        JOIN Loans l ON b.BookID = l.BookID 
                        JOIN Members m ON l.MemberID = m.MemberID
                        WHERE l.Status = 'Active' 
                        AND l.EndDate <= CURDATE() - INTERVAL 1 DAY;", connection);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var loanStartDate = reader.GetDateTime(reader.GetOrdinal("DateOfLoan")).ToString("yyyy-MM-dd");
                            var endDate = reader.GetDateTime(reader.GetOrdinal("EndDate")).ToString("yyyy-MM-dd");
                            var parsedEndDate = DateTime.Parse(endDate);
                            var daysOverdue = (DateTime.Now - parsedEndDate).Days;

                            overdueBooks.Add(new OverdueBook
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("LoanID")),
                                Title = reader.GetString(reader.GetOrdinal("BookName")),
                                Author = reader.GetString(reader.GetOrdinal("Author")),
                                BorrowerName = reader.GetString(reader.GetOrdinal("MemberName")) + " " +
                                               reader.GetString(reader.GetOrdinal("Surname")),
                                LoanStartDate = loanStartDate,
                                LoanEndDate = endDate,
                                DaysOverdue = daysOverdue > 0 ? daysOverdue : 0
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetOverdueBooks: {ex.Message}");
                throw;
            }

            return overdueBooks;
        }

        public OverdueBook GetOverdueBookById(int loanId)
        {
            try
            {
                Log.Information("Connecting to database to get overdue book details...");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand(@"
                SELECT b.BookID, b.Name AS BookName, b.Author, m.Name AS MemberName, m.Surname, l.DateOfLoan, l.EndDate, l.LoanID, l.Status AS Status 
                FROM Books b 
                JOIN Loans l ON b.BookID = l.BookID 
                JOIN Members m ON l.MemberID = m.MemberID
                WHERE l.Status = 'Active' 
                AND l.EndDate <= CURDATE() - INTERVAL 1 DAY
                AND l.LoanID = @loanId;", connection);
                    command.Parameters.AddWithValue("@loanId", loanId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var loanStartDate = reader.GetDateTime(reader.GetOrdinal("DateOfLoan")).ToString("yyyy-MM-dd");
                            var endDate = reader.GetDateTime(reader.GetOrdinal("EndDate")).ToString("yyyy-MM-dd");
                            var parsedEndDate = DateTime.Parse(endDate);
                            var daysOverdue = (DateTime.Now - parsedEndDate).Days;

                            return new OverdueBook
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("LoanID")),
                                Title = reader.GetString(reader.GetOrdinal("BookName")),
                                Author = reader.GetString(reader.GetOrdinal("Author")),
                                BorrowerName = reader.GetString(reader.GetOrdinal("MemberName")) + " " +
                                               reader.GetString(reader.GetOrdinal("Surname")),
                                LoanStartDate = loanStartDate,
                                LoanEndDate = endDate,
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                DaysOverdue = daysOverdue > 0 ? daysOverdue : 0
                            };
                        }
                        else
                        {
                            Log.Warning($"No overdue book found with LoanID: {loanId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetOverdueBookById: {ex.Message}");
                throw;
            }

            return null;
        }




        public bool EditOverdueBook(int loanId, string newEndDate = null, string status = null)
        {
            if (string.IsNullOrEmpty(newEndDate) && string.IsNullOrEmpty(status))
            {
                return false; // Niekas nepakeista
            }

            try
            {
                Log.Information("Connecting to database...");

                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Loans SET ";

                    bool firstField = true;

                    if (!string.IsNullOrEmpty(newEndDate))
                    {
                        query += "EndDate = @newEndDate ";
                        firstField = false;
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        if (!firstField)
                        {
                            query += ", ";
                        }
                        query += "Status = @status ";
                    }

                    query += "WHERE LoanID = @loanId;";

                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@loanId", loanId);

                    if (!string.IsNullOrEmpty(newEndDate))
                    {
                        command.Parameters.AddWithValue("@newEndDate", newEndDate);
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        command.Parameters.AddWithValue("@status", status);
                    }

                    int rowsAffected = command.ExecuteNonQuery();
                    Log.Information($"Successfully updated {rowsAffected} record(s).");

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in EditOverdueBook: {ex.Message}");
                throw;
            }
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
                Log.Error($"-------->Error in IsBookLoaned: {ex.Message}");
                throw;
            }
        }




        public bool UploadNewBook(BookDto newBook)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                using var command = new MySqlCommand(@"
                   INSERT INTO books (Name, Author, YearOfRelease, CategoryID, ImagePath, quantity)
                   VALUES (@Name, @Author, @YearOfRelease, @CategoryID, @ImagePath, @quantity);"

                , connection);


                command.Parameters.AddWithValue("@Name", newBook.Name);
                command.Parameters.AddWithValue("@Author", newBook.Author);
                command.Parameters.AddWithValue("@YearOfRelease", newBook.YearOfRelease);
                command.Parameters.AddWithValue("@CategoryID", newBook.CategoryId);
                command.Parameters.AddWithValue("@ImagePath", newBook.ImagePath);
                command.Parameters.AddWithValue("@quantity", newBook.Amount);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;

            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in UploadNewBook: {ex.Message}");
                throw;
            }
        }

        public bool EditBook(int bookId, BookDto updatedBook)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                using var command = new MySqlCommand(@"
            UPDATE Books 
            SET Name = IFNULL(NULLIF(@Name, ''), Name),
                Author = IFNULL(NULLIF(@Author, ''), Author),
                YearOfRelease = IFNULL(NULLIF(@YearOfRelease, 0), YearOfRelease),
                CategoryID = IFNULL(NULLIF(@CategoryID, 0), CategoryID),
                Quantity = IFNULL(NULLIF(@quantity, 0), Quantity)
            WHERE BookID = @bookId;", connection);

                command.Parameters.AddWithValue("@bookId", bookId);
                command.Parameters.AddWithValue("@Name", updatedBook.Name);
                command.Parameters.AddWithValue("@Author", updatedBook.Author);
                command.Parameters.AddWithValue("@YearOfRelease", updatedBook.YearOfRelease);
                command.Parameters.AddWithValue("@CategoryID", updatedBook.CategoryId);
                command.Parameters.AddWithValue("@quantity", updatedBook.Amount);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in EditBook: {ex.Message}");
                throw;
            }
        }

        public bool DeleteBookById(int bookId)
        {
            // Proceed with deletion
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand("DELETE FROM Books WHERE BookID = @bookId;", connection);
            command.Parameters.AddWithValue("@bookId", bookId);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }



        public bool HasActiveLoans(int bookId)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT COUNT(*) FROM Loans WHERE BookID = @bookId", connection);
            command.Parameters.AddWithValue("@bookId", bookId);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count == 0;
        }





    }
}
