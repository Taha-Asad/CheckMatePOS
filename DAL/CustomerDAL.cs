using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.DAL
{
    public class CustomerDAL : ICustomerRepository
    {
        public void AddCustomer(Customer customer)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"INSERT INTO Customers (Name, Email, Phone, TotalSpent)
                           VALUES (@name, @email, @phone, @totalSpent);";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", customer.Name);
            cmd.Parameters.AddWithValue("@email", customer.Email);
            cmd.Parameters.AddWithValue("@phone", customer.Phone);
            cmd.Parameters.AddWithValue("@totalSpent", customer.TotalSpent);
            cmd.ExecuteNonQuery();

            using var idCmd = new SqliteCommand("SELECT last_insert_rowid();", conn);
            customer.CustomerId = Convert.ToInt32(idCmd.ExecuteScalar());
        }

        public void UpdateCustomer(int customerId, Customer updatedCustomer)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"UPDATE Customers SET Name = @name, Email = @email, 
                           Phone = @phone, TotalSpent = @totalSpent 
                           WHERE CustomerId = @id;";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            cmd.Parameters.AddWithValue("@name", updatedCustomer.Name);
            cmd.Parameters.AddWithValue("@email", updatedCustomer.Email);
            cmd.Parameters.AddWithValue("@phone", updatedCustomer.Phone);
            cmd.Parameters.AddWithValue("@totalSpent", updatedCustomer.TotalSpent);
            cmd.ExecuteNonQuery();
        }

        public void DeleteCustomer(int customerId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "DELETE FROM Customers WHERE CustomerId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            cmd.ExecuteNonQuery();
        }

        public List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Customers ORDER BY CustomerId;";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(ReadCustomer(reader));
            }
            return customers;
        }

        public Customer? GetCustomerById(int customerId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Customers WHERE CustomerId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", customerId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return ReadCustomer(reader);
            }
            return null;
        }

        public Customer? GetCustomerByEmail(string email)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Customers WHERE LOWER(Email) = LOWER(@email);";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return ReadCustomer(reader);
            }
            return null;
        }

        public bool EmailExists(string email)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT COUNT(*) FROM Customers WHERE LOWER(Email) = LOWER(@email);";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);
            return (long)cmd.ExecuteScalar()! > 0;
        }

        public void UpdateTotalSpent(int customerId, decimal amount)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "UPDATE Customers SET TotalSpent = TotalSpent + @amount WHERE CustomerId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.ExecuteNonQuery();
        }

        private Customer ReadCustomer(SqliteDataReader reader)
        {
            return new Customer
            {
                CustomerId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3),
                TotalSpent = Convert.ToDecimal(reader.GetDouble(4))
            };
        }
    }
}