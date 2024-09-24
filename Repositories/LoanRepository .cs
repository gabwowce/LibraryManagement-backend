using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Serilog;
using Org.BouncyCastle.Utilities;

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

            try
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
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetLoanById: {ex.Message}");
                throw;
            }

            return null;
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            var loans = new List<Loan>();

            try
            {
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
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetAllLoans: {ex.Message}");
                throw;
            }
            return loans;
        }

        public void AddLoan(Loan loan, DateTime? customStartDate = null)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                var startDate = customStartDate ?? loan.DateOfLoan ?? DateTime.Now; 

                var endDate = startDate.AddMonths(1);

                using var command = new MySqlCommand(@"
        INSERT INTO loans (MemberID, BookID, DateOfLoan, EndDate, Status) 
        VALUES (@MemberId, @BookId, @DateOfLoan, @EndDate, @Status)", connection);

                command.Parameters.AddWithValue("@MemberId", loan.MemberId);
                command.Parameters.AddWithValue("@BookId", loan.BookId);
                command.Parameters.AddWithValue("@DateOfLoan", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@Status", loan.Status);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in AddLoan: {ex.Message}");
                throw;
            }
        }



        public void UpdateLoanStatus(int loanId, string newStatus)
        {
            try
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
            catch (Exception ex)
            {
                Log.Error($"-------->Error in UpdateLoanStatus: {ex.Message}");
                throw;
            }
        }



        public IEnumerable<Loan> GetLoansByMemberId(int memberId)
        {
            var loans = new List<Loan>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand(@"
                        SELECT * FROM Loans 
                        WHERE MemberID = @memberId
                    ", connection);
                    command.Parameters.AddWithValue("@memberId", memberId);

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
            }
            catch (Exception ex)
            {
                Log.Error($"--------> Error in GetLoansByMemberId: {ex.Message}");
                throw;
            }

            return loans;
        }


    }
}
