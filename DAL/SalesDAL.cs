using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.DAL
{
    public class SalesDAL : ISalesRepository
    {
        public string GenerateInvoiceNumber()
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT COUNT(*) FROM Invoices;";
            using var cmd = new SqliteCommand(sql, conn);
            long count = (long)cmd.ExecuteScalar()!;

            return $"INV-{DateTime.Now:yyyyMMdd}-{(count + 1):D4}";
        }

        // Lab 10: Added PaymentMethod to INSERT
        public int SaveInvoice(Invoice invoice)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"INSERT INTO Invoices (InvoiceNumber, Date, CashierId, CashierName, Subtotal, TaxAmount, Total, PaymentMethod)
                           VALUES (@invNo, @date, @cashierId, @cashierName, @subtotal, @tax, @total, @payMethod);";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@invNo", invoice.InvoiceNumber);
            cmd.Parameters.AddWithValue("@date", invoice.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@cashierId", invoice.CashierId);
            cmd.Parameters.AddWithValue("@cashierName", invoice.CashierName);
            cmd.Parameters.AddWithValue("@subtotal", invoice.Subtotal);
            cmd.Parameters.AddWithValue("@tax", invoice.TaxAmount);
            cmd.Parameters.AddWithValue("@total", invoice.Total);
            cmd.Parameters.AddWithValue("@payMethod", invoice.PaymentMethod);
            cmd.ExecuteNonQuery();

            using var idCmd = new SqliteCommand("SELECT last_insert_rowid();", conn);
            return Convert.ToInt32(idCmd.ExecuteScalar());
        }

        public void SaveInvoiceItems(int invoiceId, List<InvoiceItem> items)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"INSERT INTO InvoiceItems (InvoiceId, ProductId, ProductName, UnitPrice, Quantity, LineTotal)
                           VALUES (@invId, @prodId, @prodName, @price, @qty, @lineTotal);";

            foreach (var item in items)
            {
                using var cmd = new SqliteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@invId", invoiceId);
                cmd.Parameters.AddWithValue("@prodId", item.ProductId);
                cmd.Parameters.AddWithValue("@prodName", item.ProductName);
                cmd.Parameters.AddWithValue("@price", item.UnitPrice);
                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                cmd.Parameters.AddWithValue("@lineTotal", item.LineTotal);
                cmd.ExecuteNonQuery();
            }
        }

        // Lab 10: Read PaymentMethod
        public List<Invoice> GetAllInvoices()
        {
            var invoices = new List<Invoice>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Invoices ORDER BY InvoiceId DESC;";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                invoices.Add(ReadInvoice(reader));
            }
            return invoices;
        }

        // Lab 10: Read PaymentMethod
        public Invoice? GetInvoiceById(int invoiceId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Invoices WHERE InvoiceId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", invoiceId);

            Invoice? invoice = null;
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    invoice = ReadInvoice(reader);
                }
            }

            if (invoice == null) return null;

            string itemsSql = "SELECT * FROM InvoiceItems WHERE InvoiceId = @id;";
            using var itemsCmd = new SqliteCommand(itemsSql, conn);
            itemsCmd.Parameters.AddWithValue("@id", invoiceId);

            using var itemsReader = itemsCmd.ExecuteReader();
            while (itemsReader.Read())
            {
                invoice.Items.Add(new InvoiceItem
                {
                    InvoiceItemId = itemsReader.GetInt32(0),
                    InvoiceId = itemsReader.GetInt32(1),
                    ProductId = itemsReader.GetInt32(2),
                    ProductName = itemsReader.GetString(3),
                    UnitPrice = Convert.ToDecimal(itemsReader.GetDouble(4)),
                    Quantity = itemsReader.GetInt32(5),
                    LineTotal = Convert.ToDecimal(itemsReader.GetDouble(6))
                });
            }

            return invoice;
        }

        // Helper to read invoice from reader
        private Invoice ReadInvoice(SqliteDataReader reader)
        {
            return new Invoice
            {
                InvoiceId = reader.GetInt32(0),
                InvoiceNumber = reader.GetString(1),
                Date = DateTime.Parse(reader.GetString(2)),
                CashierId = reader.GetInt32(3),
                CashierName = reader.GetString(4),
                Subtotal = Convert.ToDecimal(reader.GetDouble(5)),
                TaxAmount = Convert.ToDecimal(reader.GetDouble(6)),
                Total = Convert.ToDecimal(reader.GetDouble(7)),
                PaymentMethod = reader.GetString(8)
            };
        }
    }
}