using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace CheckMatePOS.DAL
{
    public static class DatabaseHelper
    {
        private static readonly string DbFile = "CheckMatePOS.db";

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={DbFile}");
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        public static void InitializeDatabase()
        {
            using var conn = GetConnection();
            conn.Open();

            string usersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId       INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username     TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Role         TEXT NOT NULL CHECK(Role IN ('Admin','Cashier')),
                    IsActive     INTEGER NOT NULL DEFAULT 1
                );";

            string productsTable = @"
                CREATE TABLE IF NOT EXISTS Products (
                    ProductId       INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductName     TEXT NOT NULL UNIQUE,
                    Price           REAL NOT NULL CHECK(Price > 0),
                    QuantityInStock INTEGER NOT NULL DEFAULT 0 CHECK(QuantityInStock >= 0)
                );";

            // Lab 11: Added Customers table
            string customersTable = @"
                CREATE TABLE IF NOT EXISTS Customers (
                    CustomerId  INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name        TEXT NOT NULL,
                    Email       TEXT NOT NULL UNIQUE,
                    Phone       TEXT DEFAULT '',
                    TotalSpent  REAL NOT NULL DEFAULT 0
                );";

            // Lab 11: Modified Invoices to include customer info
            string invoicesTable = @"
                CREATE TABLE IF NOT EXISTS Invoices (
                    InvoiceId     INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceNumber TEXT NOT NULL UNIQUE,
                    Date          TEXT NOT NULL,
                    CashierId     INTEGER NOT NULL,
                    CashierName   TEXT NOT NULL,
                    CustomerId    INTEGER,
                    CustomerName  TEXT DEFAULT 'Walk-in',
                    Subtotal      REAL NOT NULL,
                    TaxAmount     REAL NOT NULL,
                    Total         REAL NOT NULL,
                    PaymentMethod TEXT NOT NULL DEFAULT 'Cash'
                );";

            string invoiceItemsTable = @"
                CREATE TABLE IF NOT EXISTS InvoiceItems (
                    InvoiceItemId INTEGER PRIMARY KEY AUTOINCREMENT,
                    InvoiceId     INTEGER NOT NULL,
                    ProductId     INTEGER NOT NULL,
                    ProductName   TEXT NOT NULL,
                    UnitPrice     REAL NOT NULL,
                    Quantity      INTEGER NOT NULL,
                    LineTotal     REAL NOT NULL
                );";

            using (var cmd = new SqliteCommand(usersTable, conn)) cmd.ExecuteNonQuery();
            using (var cmd = new SqliteCommand(productsTable, conn)) cmd.ExecuteNonQuery();
            using (var cmd = new SqliteCommand(customersTable, conn)) cmd.ExecuteNonQuery();  // ← NEW
            using (var cmd = new SqliteCommand(invoicesTable, conn)) cmd.ExecuteNonQuery();
            using (var cmd = new SqliteCommand(invoiceItemsTable, conn)) cmd.ExecuteNonQuery();

            SeedDefaultAdmin(conn);
            SeedDefaultCustomers(conn);  // ← NEW
            Console.WriteLine("[DB] Database initialized successfully.");
        }

        private static void SeedDefaultAdmin(SqliteConnection conn)
        {
            string checkSql = "SELECT COUNT(*) FROM Users;";
            using var checkCmd = new SqliteCommand(checkSql, conn);
            long count = (long)checkCmd.ExecuteScalar()!;

            if (count == 0)
            {
                string insertSql = @"INSERT INTO Users (Username, PasswordHash, Role, IsActive)
                                     VALUES ('admin', @hash, 'Admin', 1);";
                using var insertCmd = new SqliteCommand(insertSql, conn);
                insertCmd.Parameters.AddWithValue("@hash", HashPassword("admin123"));
                insertCmd.ExecuteNonQuery();
                Console.WriteLine("[DB] Default admin created (username: admin, password: admin123)");
            }
        }

        // Lab 11: Seed sample customers
        private static void SeedDefaultCustomers(SqliteConnection conn)
        {
            string checkSql = "SELECT COUNT(*) FROM Customers;";
            using var checkCmd = new SqliteCommand(checkSql, conn);
            long count = (long)checkCmd.ExecuteScalar()!;

            if (count == 0)
            {
                string insertSql = @"INSERT INTO Customers (Name, Email, Phone, TotalSpent)
                                     VALUES (@name, @email, @phone, @spent);";

                // Sample customer 1
                using (var cmd = new SqliteCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", "Alice Johnson");
                    cmd.Parameters.AddWithValue("@email", "alice@example.com");
                    cmd.Parameters.AddWithValue("@phone", "555-1234");
                    cmd.Parameters.AddWithValue("@spent", 0);
                    cmd.ExecuteNonQuery();
                }

                // Sample customer 2
                using (var cmd = new SqliteCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", "Bob Smith");
                    cmd.Parameters.AddWithValue("@email", "bob@example.com");
                    cmd.Parameters.AddWithValue("@phone", "555-5678");
                    cmd.Parameters.AddWithValue("@spent", 0);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("[DB] Sample customers created.");
            }
        }
    }
}