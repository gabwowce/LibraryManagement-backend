using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly string _connectionString;

        public LoanRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Loan GetLoanById(int id)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Loans WHERE LoanID = @id", connection);
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Loan
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("LoanID")),
                            MemberId = reader.GetInt32(reader.GetOrdinal("MemberID")),
                            BookId = reader.GetInt32(reader.GetOrdinal("BookID")),
                            DateOfLoan = reader.GetDateTime(reader.GetOrdinal("DateOfLoan")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            Status = reader.GetString(reader.GetOrdinal("Status"))
                        };
                    }
                }
            }
            return null;
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            var loans = new List<Loan>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM Loans", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        loans.Add(new Loan
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("LoanID")),
                            MemberId = reader.GetInt32(reader.GetOrdinal("MemberID")),
                            BookId = reader.GetInt32(reader.GetOrdinal("BookID")),
                            DateOfLoan = reader.GetDateTime(reader.GetOrdinal("DateOfLoan")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            Status = reader.GetString(reader.GetOrdinal("Status"))
                        });
                    }
                }
            }
            return loans;
        }

        public void AddLoan(Loan loan, DateTime? customStartDate = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(@"
                INSERT INTO loans (MemberID, BookID, DateOfLoan, EndDate, Status) 
                VALUES (@MemberId, @BookId, @DateOfLoan, @EndDate, @Status)", connection);

            command.Parameters.AddWithValue("@MemberId", loan.MemberId);
            command.Parameters.AddWithValue("@BookId", loan.BookId);
            command.Parameters.AddWithValue("@DateOfLoan", customStartDate ?? DateTime.Now);
            command.Parameters.AddWithValue("@EndDate", (customStartDate ?? DateTime.Now).AddMonths(1));
            command.Parameters.AddWithValue("@Status", loan.Status);

            command.ExecuteNonQuery();
        }

        public void UpdateLoanStatus(int loanId, string newStatus)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand(@"
                UPDATE loans
                SET Status = @Status
                WHERE LoanID = @LoanId", connection);

            command.Parameters.AddWithValue("@Status", newStatus);
            command.Parameters.AddWithValue("@LoanId", loanId);

            command.ExecuteNonQuery();
        }

    }
}
