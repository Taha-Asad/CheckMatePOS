using System.Collections.Generic;
using CheckMatePOS.Models;

namespace CheckMatePOS.Interfaces
{
    public interface IProductRepository
    {
        void AddProduct(Product product);
        void UpdateProduct(int productId, Product updatedProduct);
        void DeleteProduct(int productId);
        List<Product> GetAllProducts();
        Product? GetProductById(int productId);
        bool ProductNameExists(string productName);
        void UpdateStock(int productId, int newQuantity);
        List<Product> SearchProducts(string searchTerm);
    }
}