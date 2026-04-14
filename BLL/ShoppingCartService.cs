using System;
using System.Collections.Generic;
using System.Linq;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class ShoppingCartService
    {
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;
        private const decimal TAX_RATE = 0.17m;

        public ShoppingCartService(IProductRepository productRepo, ICartRepository cartRepo)
        {
            productRepository = productRepo;
            cartRepository = cartRepo;
        }

        // Lab 9: Search products by name
        public void SearchAndDisplayProducts(string searchTerm)
        {
            var products = productRepository.SearchProducts(searchTerm);

            if (products.Count == 0)
            {
                Console.WriteLine($"  No products found for '{searchTerm}'.");
                return;
            }

            Console.WriteLine($"\n  Search results for '{searchTerm}':");
            Console.WriteLine($"  {"ID",-6} {"Name",-20} {"Price",-12} {"Stock",-10}");
            Console.WriteLine("  " + new string('-', 50));

            foreach (var p in products)
            {
                string stock = p.QuantityInStock > 0 ? p.QuantityInStock.ToString() : "OUT OF STOCK";
                Console.WriteLine($"  {p.ProductId,-6} {p.ProductName,-20} {p.Price,-12:F2} {stock,-10}");
            }
        }

        // Lab 9: Display all products
        public void DisplayProductList()
        {
            var products = productRepository.GetAllProducts();

            if (products.Count == 0)
            {
                Console.WriteLine("  No products available.");
                return;
            }

            Console.WriteLine($"\n  {"ID",-6} {"Product",-20} {"Price",-12} {"Stock",-10}");
            Console.WriteLine("  " + new string('-', 50));

            foreach (var p in products)
            {
                string stock = p.QuantityInStock > 0 ? p.QuantityInStock.ToString() : "OUT OF STOCK";
                Console.WriteLine($"  {p.ProductId,-6} {p.ProductName,-20} {p.Price,-12:F2} {stock,-10}");
            }
        }

        // Lab 9: Add to cart with stock check
        public void AddProductToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("  [Error] Quantity must be at least 1!");
                return;
            }

            var product = productRepository.GetProductById(productId);
            if (product == null)
            {
                Console.WriteLine("  [Error] Product not found!");
                return;
            }

            var cartItems = cartRepository.GetCartItems();
            var existing = cartItems.FirstOrDefault(c => c.product.ProductId == productId);
            int alreadyInCart = existing.product != null ? existing.quantity : 0;

            if (product.QuantityInStock < (alreadyInCart + quantity))
            {
                Console.WriteLine($"  [Error] Not enough stock! Available: {product.QuantityInStock}, In cart: {alreadyInCart}");
                return;
            }

            cartRepository.AddToCart(product, quantity);
            Console.WriteLine($"  Added {quantity}x {product.ProductName} to cart.");
        }

        // Lab 9: Remove from cart
        public void RemoveProductFromCart(int productId)
        {
            var cartItems = cartRepository.GetCartItems();
            var item = cartItems.FirstOrDefault(c => c.product.ProductId == productId);

            if (item.product == null)
            {
                Console.WriteLine("  [Error] Product not in cart!");
                return;
            }

            cartRepository.RemoveFromCart(productId);
            Console.WriteLine($"  Removed {item.product.ProductName} from cart.");
        }

        // Lab 9: Update quantity in cart
        public void UpdateCartItemQuantity(int productId, int newQuantity)
        {
            var cartItems = cartRepository.GetCartItems();
            var item = cartItems.FirstOrDefault(c => c.product.ProductId == productId);

            if (item.product == null)
            {
                Console.WriteLine("  [Error] Product not in cart!");
                return;
            }

            if (newQuantity <= 0)
            {
                cartRepository.RemoveFromCart(productId);
                Console.WriteLine($"  Removed {item.product.ProductName} from cart.");
                return;
            }

            var product = productRepository.GetProductById(productId);
            if (product != null && product.QuantityInStock < newQuantity)
            {
                Console.WriteLine($"  [Error] Not enough stock! Available: {product.QuantityInStock}");
                return;
            }

            cartRepository.UpdateQuantity(productId, newQuantity);
            Console.WriteLine($"  Updated {item.product.ProductName} quantity to {newQuantity}.");
        }

        // Lab 9: View cart with totals
        public void ViewCart()
        {
            var cartItems = cartRepository.GetCartItems();

            if (!cartItems.Any())
            {
                Console.WriteLine("  Cart is empty.");
                return;
            }

            Console.WriteLine($"\n  {"#",-4} {"Product",-20} {"Price",-10} {"Qty",-6} {"Total",-10}");
            Console.WriteLine("  " + new string('-', 52));

            int num = 1;
            foreach (var item in cartItems)
            {
                decimal lineTotal = item.product.Price * item.quantity;
                Console.WriteLine($"  {num,-4} {item.product.ProductName,-20} {item.product.Price,-10:F2} {item.quantity,-6} {lineTotal,-10:F2}");
                num++;
            }

            Console.WriteLine("  " + new string('-', 52));
            Console.WriteLine($"  {"Subtotal:",-36} {GetSubtotal():F2}");
            Console.WriteLine($"  {"Tax (17%):",-36} {GetTaxAmount():F2}");
            Console.WriteLine($"  {"TOTAL:",-36} {GetTotal():F2}");
        }

        public decimal GetSubtotal() => cartRepository.GetCartTotal();
        public decimal GetTaxAmount() => Math.Round(GetSubtotal() * TAX_RATE, 2);
        public decimal GetTotal() => GetSubtotal() + GetTaxAmount();
        public bool IsCartEmpty() => cartRepository.IsEmpty();

        public void ClearCart()
        {
            cartRepository.ClearCart();
        }

        public void CancelTransaction()
        {
            cartRepository.ClearCart();
            Console.WriteLine("  Transaction cancelled. Cart cleared.");
        }

        public void NewTransaction()
        {
            cartRepository.ClearCart();
            Console.WriteLine("  New transaction started.");
        }

        // Lab 11: Modified checkout to accept customer info
        public Invoice? Checkout(int cashierId, string cashierName, string paymentMethod,
                                 int? customerId, string customerName,
                                 ISalesRepository salesRepo, ProductService productService)
        {
            var cartItems = cartRepository.GetCartItems();

            if (!cartItems.Any())
            {
                Console.WriteLine("  [Error] Cart is empty!");
                return null;
            }

            foreach (var item in cartItems)
            {
                productService.ReduceStock(item.product.ProductId, item.quantity);
            }

            string invoiceNumber = salesRepo.GenerateInvoiceNumber();

            var invoice = new Invoice
            {
                InvoiceNumber = invoiceNumber,
                Date = DateTime.Now,
                CashierId = cashierId,
                CashierName = cashierName,
                CustomerId = customerId,          // ← NEW
                CustomerName = customerName,       // ← NEW
                Subtotal = GetSubtotal(),
                TaxAmount = GetTaxAmount(),
                Total = GetTotal(),
                PaymentMethod = paymentMethod
            };

            int invoiceId = salesRepo.SaveInvoice(invoice);
            invoice.InvoiceId = invoiceId;

            var invoiceItems = new List<InvoiceItem>();
            foreach (var cartItem in cartItems)
            {
                invoiceItems.Add(new InvoiceItem
                {
                    InvoiceId = invoiceId,
                    ProductId = cartItem.product.ProductId,
                    ProductName = cartItem.product.ProductName,
                    UnitPrice = cartItem.product.Price,
                    Quantity = cartItem.quantity,
                    LineTotal = cartItem.product.Price * cartItem.quantity
                });
            }

            salesRepo.SaveInvoiceItems(invoiceId, invoiceItems);
            invoice.Items = invoiceItems;

            cartRepository.ClearCart();

            return invoice;
        }
    }
}


