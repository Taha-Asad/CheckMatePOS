using System;
using System.Collections.Generic;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class ProductService
    {
        private readonly IProductRepository productRepo;

        public ProductService(IProductRepository repository)
        {
            productRepo = repository;
        }

        public bool AddProduct(string name, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("  [Error] Product name cannot be empty!");
                return false;
            }
            if (price <= 0)
            {
                Console.WriteLine("  [Error] Price must be greater than 0!");
                return false;
            }
            if (quantity < 0)
            {
                Console.WriteLine("  [Error] Quantity cannot be negative!");
                return false;
            }
            if (productRepo.ProductNameExists(name))
            {
                Console.WriteLine("  [Error] Product with this name already exists!");
                return false;
            }

            var product = new Product
            {
                ProductName = name,
                Price = price,
                QuantityInStock = quantity
            };

            productRepo.AddProduct(product);
            Console.WriteLine($"  Product '{name}' added! (ID: {product.ProductId})");
            return true;
        }

        public bool UpdateProduct(int id, string name, decimal price, int quantity)
        {
            var existing = productRepo.GetProductById(id);
            if (existing == null)
            {
                Console.WriteLine("  [Error] Product not found!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(name)) name = existing.ProductName;
            if (price <= 0) price = existing.Price;
            if (quantity < 0) quantity = existing.QuantityInStock;

            var updated = new Product
            {
                ProductName = name,
                Price = price,
                QuantityInStock = quantity
            };

            productRepo.UpdateProduct(id, updated);
            Console.WriteLine($"  Product ID {id} updated!");
            return true;
        }

        public bool DeleteProduct(int id)
        {
            var product = productRepo.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine("  [Error] Product not found!");
                return false;
            }

            productRepo.DeleteProduct(id);
            Console.WriteLine($"  Product '{product.ProductName}' deleted!");
            return true;
        }

        public List<Product> GetAllProducts()
        {
            return productRepo.GetAllProducts();
        }

        public Product? GetProductById(int id)
        {
            return productRepo.GetProductById(id);
        }

        public bool HasEnoughStock(int productId, int quantity)
        {
            var product = productRepo.GetProductById(productId);
            if (product == null) return false;
            return product.QuantityInStock >= quantity;
        }

        public void ReduceStock(int productId, int quantitySold)
        {
            var product = productRepo.GetProductById(productId);
            if (product != null)
            {
                int newQty = product.QuantityInStock - quantitySold;
                productRepo.UpdateStock(productId, newQty);
            }
        }

        public void RestoreStock(int productId, int quantity)
        {
            var product = productRepo.GetProductById(productId);
            if (product != null)
            {
                int newQty = product.QuantityInStock + quantity;
                productRepo.UpdateStock(productId, newQty);
            }
        }
    }
}