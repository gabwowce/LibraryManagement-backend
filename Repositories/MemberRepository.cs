﻿using System.Collections.Generic;
using MySql.Data.MySqlClient;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Serilog;

namespace LibraryManagement.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly string _connectionString;

        public MemberRepository(string connectionString)
        {
            _connectionString = connectionString;
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
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetMemberById: {ex.Message}");
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
                                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"-------->Error in GetAllMembers: {ex.Message}");
                throw;
            }

            return members;
        }
    }
}
