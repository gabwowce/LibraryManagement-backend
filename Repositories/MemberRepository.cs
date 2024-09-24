using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Serilog;
using System.Linq;
using LibraryManagement.Helpers;

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


        public MemberDto GetByUsername(string username)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand("SELECT * FROM Members WHERE Username = @Username", connection);
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MemberDto
                            {
                                Name = reader["Name"].ToString(),
                                Surname = reader["Surname"].ToString(),
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")).ToString("yyyy-MM-dd"),
                                PhoneNumber = reader["phone_number"].ToString(),
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"----> Error retrieving member by username: {ex.Message}");
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

        public IEnumerable<Member> GetAdminAndManagerMembers()
        {
            var members = new List<Member>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand("SELECT * FROM Members WHERE Role IN ('admin', 'manager')", connection);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            members.Add(new Member
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("MemberID")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Role = reader.GetString(reader.GetOrdinal("Role")),
                                
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
                Log.Error($"--------> Error in GetAdminAndManagerMembers: {ex.Message}");
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

                    string hashedPassword = PasswordHelper.HashPassword(newMember.Surname);

                    var command = new MySqlCommand(@"INSERT INTO Members (Name, Surname, DateOfBirth, phone_number, Username, Password, Role) 
                VALUES (@name, @surname, @dateOfBirth, @phoneNumber, @Username, @Password, @Role)", connection);

                    command.Parameters.AddWithValue("@name", newMember.Name);
                    command.Parameters.AddWithValue("@surname", newMember.Surname);
                    command.Parameters.AddWithValue("@dateOfBirth", newMember.DateOfBirth);
                    command.Parameters.AddWithValue("@phoneNumber", newMember.PhoneNumber);

                    command.Parameters.AddWithValue("@Username", newMember.Name);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Role", "member");


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

        public bool CreateManager(MemberDto newMember)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string hashedPassword = PasswordHelper.HashPassword(newMember.Surname);

                    var command = new MySqlCommand(@"INSERT INTO Members (Name, Surname, DateOfBirth, phone_number, Username, Password, Role) 
                VALUES (@name, @surname, @dateOfBirth, @phoneNumber, @Username, @Password, @Role)", connection);

                    command.Parameters.AddWithValue("@name", newMember.Name);
                    command.Parameters.AddWithValue("@surname", newMember.Surname);
                    command.Parameters.AddWithValue("@dateOfBirth", newMember.DateOfBirth);
                    command.Parameters.AddWithValue("@phoneNumber", newMember.PhoneNumber);

                    command.Parameters.AddWithValue("@Username", newMember.Name);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Role", "manager");


                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"--------> Error in CreateManager: {ex.Message}");
                throw;
            }
        }

        public bool CreateAdmin(MemberDto newAdmin)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string hashedPassword = PasswordHelper.HashPassword(newAdmin.Surname);

                    var command = new MySqlCommand(@"INSERT INTO Members (Name, Surname, DateOfBirth, phone_number, Username, Password, Role) 
                VALUES (@name, @surname, @dateOfBirth, @phoneNumber, @Username, @Password, @Role)", connection);

                    command.Parameters.AddWithValue("@name", newAdmin.Name);
                    command.Parameters.AddWithValue("@surname", newAdmin.Surname);
                    command.Parameters.AddWithValue("@dateOfBirth", newAdmin.DateOfBirth);
                    command.Parameters.AddWithValue("@phoneNumber", newAdmin.PhoneNumber);

                    command.Parameters.AddWithValue("@Username", newAdmin.Name);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Role", "admin");


                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"--------> Error in CreateAdmin: {ex.Message}");
                throw;
            }
        }

        public bool HasActiveLoans(int memberId)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                using var command = new MySqlCommand("SELECT COUNT(*) FROM Loans WHERE MemberID = @memberId", connection);
                command.Parameters.AddWithValue("@memberId", memberId);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count == 0;
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"------> Error checking active loans: {ex.Message}");
                return false; 
            }
        }

        public bool DeleteMemberById(int memberId)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();

                using var command = new MySqlCommand("DELETE FROM Members WHERE MemberID = @memberId AND role != 'admin';", connection);
                command.Parameters.AddWithValue("@memberId", memberId);

                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"------>  Error deleting member: {ex.Message}");
                return false; 
            }
        }





    }
}
