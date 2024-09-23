using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Serilog;
using System.Linq;

namespace LibraryManagement.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly string _connectionString;
        private readonly ILoanRepository _loanRepository;

        public MemberRepository(string connectionString, ILoanRepository loanRepository)
        {
            _connectionString = connectionString;
            _loanRepository = loanRepository;
        }

        public Member GetMemberById(int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand("SELECT * FROM Members WHERE MemberID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Member
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("MemberID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")).ToString("yyyy-MM-dd"),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                                Loans = _loanRepository.GetLoansByMemberId(id).ToList()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"--------> Error in GetMemberById: {ex.Message}");
                throw;
            }

            return null;
        }

        public IEnumerable<Member> GetAllMembers()
        {
            var members = new List<Member>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand("SELECT * FROM Members", connection);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            members.Add(new Member
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("MemberID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")).ToString("yyyy-MM-dd"),
                                PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                                Loans = _loanRepository.GetLoansByMemberId(reader.GetInt32(reader.GetOrdinal("MemberID"))).ToList()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"--------> Error in GetAllMembers: {ex.Message}");
                throw;
            }

            return members;
        }

        public bool UpdateMember(MemberDto updatedMember, int id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new MySqlCommand(@"UPDATE Members SET 
                Name = @name, 
                Surname = @surname, 
                DateOfBirth = @dateOfBirth, 
                phone_number = @phoneNumber 
                WHERE MemberID = @id", connection);

                    command.Parameters.AddWithValue("@name", updatedMember.Name);
                    command.Parameters.AddWithValue("@surname", updatedMember.Surname);
                    command.Parameters.AddWithValue("@dateOfBirth", updatedMember.DateOfBirth);
                    command.Parameters.AddWithValue("@phoneNumber", updatedMember.PhoneNumber);
                    command.Parameters.AddWithValue("@id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"--------> Error in UpdateMember: {ex.Message}");
                throw;
            }
        }


        public bool CreateMember(MemberDto newMember)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new MySqlCommand(@"INSERT INTO Members (Name, Surname, DateOfBirth, phone_number) 
                VALUES (@name, @surname, @dateOfBirth, @phoneNumber)", connection);

                    command.Parameters.AddWithValue("@name", newMember.Name);
                    command.Parameters.AddWithValue("@surname", newMember.Surname);
                    command.Parameters.AddWithValue("@dateOfBirth", newMember.DateOfBirth);
                    command.Parameters.AddWithValue("@phoneNumber", newMember.PhoneNumber);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"--------> Error in CreateMember: {ex.Message}");
                throw;
            }
        }

        public bool HasActiveLoans(int memberId)
        {
            // Check if the member has any active loans
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand("SELECT COUNT(*) FROM Loans WHERE MemberID = @memberId", connection);
            command.Parameters.AddWithValue("@memberId", memberId);

            int count = Convert.ToInt32(command.ExecuteScalar());
            return count == 0;
        }

        public bool DeleteMemberById(int memberId)
        {
            
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var command = new MySqlCommand("DELETE FROM Members WHERE MemberID = @memberId;", connection);
            command.Parameters.AddWithValue("@memberId", memberId);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }





    }
}
