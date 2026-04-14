using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.DAL
{
    public class ProductDAL : IProductRepository
    {
        public void AddProduct(Product product)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"INSERT INTO Products (ProductName, Price, QuantityInStock)
                           VALUES (@name, @price, @qty);";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", product.ProductName);
            cmd.Parameters.AddWithValue("@price", product.Price);
            cmd.Parameters.AddWithValue("@qty", product.QuantityInStock);
            cmd.ExecuteNonQuery();

            using var idCmd = new SqliteCommand("SELECT last_insert_rowid();", conn);
            product.ProductId = Convert.ToInt32(idCmd.ExecuteScalar());
        }

        public void UpdateProduct(int productId, Product updatedProduct)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = @"UPDATE Products SET ProductName = @name, Price = @price,
                           QuantityInStock = @qty WHERE ProductId = @id;";

            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", productId);
            cmd.Parameters.AddWithValue("@name", updatedProduct.ProductName);
            cmd.Parameters.AddWithValue("@price", updatedProduct.Price);
            cmd.Parameters.AddWithValue("@qty", updatedProduct.QuantityInStock);
            cmd.ExecuteNonQuery();
        }

        public void DeleteProduct(int productId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "DELETE FROM Products WHERE ProductId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", productId);
            cmd.ExecuteNonQuery();
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Products ORDER BY ProductId;";
            using var cmd = new SqliteCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                products.Add(ReadProduct(reader));
            }
            return products;
        }

        public Product? GetProductById(int productId)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Products WHERE ProductId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", productId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return ReadProduct(reader);
            }
            return null;
        }

        public bool ProductNameExists(string productName)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT COUNT(*) FROM Products WHERE LOWER(ProductName) = LOWER(@name);";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", productName);
            return (long)cmd.ExecuteScalar()! > 0;
        }

        public void UpdateStock(int productId, int newQuantity)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "UPDATE Products SET QuantityInStock = @qty WHERE ProductId = @id;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@qty", newQuantity);
            cmd.Parameters.AddWithValue("@id", productId);
            cmd.ExecuteNonQuery();
        }

        // Lab 9: Search products by name
        public List<Product> SearchProducts(string searchTerm)
        {
            var products = new List<Product>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Products WHERE LOWER(ProductName) LIKE LOWER(@term) ORDER BY ProductId;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@term", $"%{searchTerm}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                products.Add(ReadProduct(reader));
            }
            return products;
        }

        // Helper to read product from reader
        private Product ReadProduct(SqliteDataReader reader)
        {
            return new Product
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                Price = Convert.ToDecimal(reader.GetDouble(2)),
                QuantityInStock = reader.GetInt32(3)
            };
        }
    }
}