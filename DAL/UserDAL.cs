using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.DAL
{
    public class UserDAL : IUserRepository
    {
        public User? GetUserByUsername(string username)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Users WHERE Username = @username;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Role = reader.GetString(3),
                    IsActive = reader.GetInt32(4) == 1
                };
            }
            return null;
        }

        public void CreateUser(User user)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"INSERT INTO Users (Username, PasswordHash, Role, IsActive)
                           VALUES (@username, @hash, @role, @active);";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@hash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@role", user.Role);
            cmd.Parameters.AddWithValue("@active", user.IsActive ? 1 : 0);
            cmd.ExecuteNonQuery();
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Users ORDER BY UserId;";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new User
                {
                    UserId = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Role = reader.GetString(3),
                    IsActive = reader.GetInt32(4) == 1
                });
            }
            return users;
        }

        public void SetUserActive(int userId, bool isActive)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "UPDATE Users SET IsActive = @active WHERE UserId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@active", isActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }

        public bool UsernameExists(string username)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT COUNT(*) FROM Users WHERE Username = @username;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            return (long)cmd.ExecuteScalar()! > 0;
        }
    }
}