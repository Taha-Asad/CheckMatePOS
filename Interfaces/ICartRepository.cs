using System.Collections.Generic;
using CheckMatePOS.Models;

namespace CheckMatePOS.Interfaces
{
    public interface ICartRepository
    {
        void AddToCart(Product product, int quantity);
        void RemoveFromCart(int productId);
        void UpdateQuantity(int productId, int newQuantity);
        List<(Product product, int quantity)> GetCartItems();
        decimal GetCartTotal();
        void ClearCart();
        bool IsEmpty();
    }
}