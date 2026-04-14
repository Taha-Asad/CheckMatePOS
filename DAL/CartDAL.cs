using System.Collections.Generic;
using System.Linq;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.DAL
{
    public class CartDAL : ICartRepository
    {
        private List<(Product product, int quantity)> cartItems = new List<(Product product, int quantity)>();

        public void AddToCart(Product product, int quantity)
        {
            var existingItem = cartItems.FirstOrDefault(c => c.product.ProductId == product.ProductId);

            if (existingItem.product != null)
            {
                cartItems.Remove(existingItem);
                cartItems.Add((product, existingItem.quantity + quantity));
            }
            else
            {
                cartItems.Add((product, quantity));
            }
        }

        public void RemoveFromCart(int productId)
        {
            var item = cartItems.FirstOrDefault(c => c.product.ProductId == productId);
            if (item.product != null)
            {
                cartItems.Remove(item);
            }
        }

        public void UpdateQuantity(int productId, int newQuantity)
        {
            var item = cartItems.FirstOrDefault(c => c.product.ProductId == productId);
            if (item.product != null)
            {
                cartItems.Remove(item);
                if (newQuantity > 0)
                {
                    cartItems.Add((item.product, newQuantity));
                }
            }
        }

        public List<(Product product, int quantity)> GetCartItems()
        {
            return cartItems;
        }

        public decimal GetCartTotal()
        {
            return cartItems.Sum(c => c.product.Price * c.quantity);
        }

        public void ClearCart()
        {
            cartItems.Clear();
        }

        public bool IsEmpty()
        {
            return cartItems.Count == 0;
        }
    }
}