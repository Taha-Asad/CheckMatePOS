using System;
using CheckMatePOS.BLL;
using CheckMatePOS.DAL;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.UI
{
    public class CashierMenu
    {
        private readonly AuthService authService;
        private readonly ProductService productService;
        private readonly ShoppingCartService shoppingCartService;
        private readonly ISalesRepository salesRepo;

        public CashierMenu(AuthService auth, ProductService product, ShoppingCartService shopping, ISalesRepository salesRepository)
        {
            authService = auth;
            productService = product;
            shoppingCartService = shopping;
            salesRepo = salesRepository;
        }

        public void Show()
        {
            shoppingCartService.NewTransaction();

            bool running = true;
            while (running)
            {
                Console.WriteLine("            CASHIER TERMINAL                ");
                Console.WriteLine("  1.  View All Products                     ");
                Console.WriteLine("  2.  Search Products                       ");
                Console.WriteLine("  3.  Add Item to Cart                      ");
                Console.WriteLine("  4.  View Cart                             ");
                Console.WriteLine("  5.  Update Item Quantity in Cart          ");
                Console.WriteLine("  6.  Remove Item from Cart                 ");
                Console.WriteLine("  7.  Checkout & Pay                        ");
                Console.WriteLine("  8.  Cancel Transaction                    ");
                Console.WriteLine("  9.  New Transaction                       ");
                Console.WriteLine("  10. View Past Invoices                    ");
                Console.WriteLine("  0.  Logout                                ");
                Console.Write("  Choice: ");

                switch (Console.ReadLine())
                {
                    case "1": ViewProducts(); break;
                    case "2": SearchProducts(); break;
                    case "3": AddToCart(); break;
                    case "4": ViewCart(); break;
                    case "5": UpdateCartItem(); break;
                    case "6": RemoveFromCart(); break;
                    case "7": Checkout(); break;
                    case "8": CancelTransaction(); break;
                    case "9": NewTransaction(); break;
                    case "10": ViewPastInvoices(); break;
                    case "0":
                        if (!shoppingCartService.IsCartEmpty())
                        {
                            Console.Write("  Cart has items. Cancel and logout? (y/n): ");
                            if (Console.ReadLine()?.ToLower() != "y") break;
                            shoppingCartService.CancelTransaction();
                        }
                        authService.Logout();
                        running = false;
                        break;
                    default:
                        Console.WriteLine("  Invalid choice!");
                        break;
                }
            }
        }

        private void ViewProducts()
        {
            shoppingCartService.DisplayProductList();
        }

        private void SearchProducts()
        {
            Console.Write("\n  Enter search term: ");
            string searchTerm = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("  Please enter a search term.");
                return;
            }

            shoppingCartService.SearchAndDisplayProducts(searchTerm);
        }

        private void AddToCart()
        {
            ViewProducts();
            Console.Write("\n  Enter Product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            Console.Write("  Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty))
            { Console.WriteLine("  Invalid quantity!"); return; }

            shoppingCartService.AddProductToCart(id, qty);
            shoppingCartService.ViewCart();
        }

        private void ViewCart()
        {
            shoppingCartService.ViewCart();
        }

        private void UpdateCartItem()
        {
            if (shoppingCartService.IsCartEmpty())
            { Console.WriteLine("  Cart is empty!"); return; }

            shoppingCartService.ViewCart();
            Console.Write("\n  Enter Product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            Console.Write("  New Quantity (0 to remove): ");
            if (!int.TryParse(Console.ReadLine(), out int qty))
            { Console.WriteLine("  Invalid quantity!"); return; }

            shoppingCartService.UpdateCartItemQuantity(id, qty);
            shoppingCartService.ViewCart();
        }

        private void RemoveFromCart()
        {
            if (shoppingCartService.IsCartEmpty())
            { Console.WriteLine("  Cart is empty!"); return; }

            shoppingCartService.ViewCart();
            Console.Write("\n  Enter Product ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            shoppingCartService.RemoveProductFromCart(id);
            shoppingCartService.ViewCart();
        }

        // ══════════════════════════════════════════════
        //  Lab 10: CHECKOUT WITH PAYMENT
        // ══════════════════════════════════════════════
        // Lab 11: Modified checkout to capture customer
        private void Checkout()
        {
            if (shoppingCartService.IsCartEmpty())
            {
                Console.WriteLine("  Cart is empty! Add items first.");
                return;
            }

            Console.WriteLine("\n  ══════════ ORDER SUMMARY ══════════");
            shoppingCartService.ViewCart();

            // Lab 11: Ask for customer (optional)
            Console.WriteLine("\n  ┌─────── Customer Information ───────┐");
            Console.Write("  Enter customer email (or press Enter to skip): ");
            string customerEmail = Console.ReadLine() ?? "";

            int? customerId = null;
            string customerName = "Walk-in";

            if (!string.IsNullOrWhiteSpace(customerEmail))
            {
                var customerService = new CustomerService(new CustomerDAL());
                var customer = customerService.GetCustomerByEmail(customerEmail);

                if (customer != null)
                {
                    customerId = customer.CustomerId;
                    customerName = customer.Name;
                    Console.WriteLine($"  Found: {customer.Name}");
                }
                else
                {
                    Console.Write("  Customer not found. Register new? (y/n): ");
                    if (Console.ReadLine()?.ToLower() == "y")
                    {
                        Console.Write("  Name: ");
                        string name = Console.ReadLine() ?? "";
                        Console.Write("  Phone: ");
                        string phone = Console.ReadLine() ?? "";

                        if (customerService.AddCustomer(name, customerEmail, phone))
                        {
                            var newCustomer = customerService.GetCustomerByEmail(customerEmail);
                            if (newCustomer != null)
                            {
                                customerId = newCustomer.CustomerId;
                                customerName = newCustomer.Name;
                            }
                        }
                    }
                }
            }

            // Select payment method
            Console.WriteLine("\n  ┌─────── Select Payment Method ───────┐");
            Console.WriteLine("  │  1. Cash                             │");
            Console.WriteLine("  │  2. Credit Card                      │");
            Console.WriteLine("  │  3. PayPal                           │");
            Console.WriteLine("  │  0. Cancel Checkout                  │");
            Console.WriteLine("  └─────────────────────────────────────┘");
            Console.Write("  Choice: ");

            string paymentChoice = Console.ReadLine() ?? "";
            string paymentMethodName = "";

            switch (paymentChoice)
            {
                case "1":
                    paymentMethodName = "Cash";
                    if (!ProcessCashPayment()) return;
                    break;
                case "2":
                    paymentMethodName = "Credit Card";
                    if (!ProcessCreditCardPayment()) return;
                    break;
                case "3":
                    paymentMethodName = "PayPal";
                    if (!ProcessPayPalPayment()) return;
                    break;
                case "0":
                    Console.WriteLine("  Checkout cancelled.");
                    return;
                default:
                    Console.WriteLine("  Invalid payment method!");
                    return;
            }

            // Lab 11: Pass customer info to checkout
            var invoice = shoppingCartService.Checkout(
                authService.CurrentUser!.UserId,
                authService.CurrentUser.Username,
                paymentMethodName,
                customerId,              // ← NEW
                customerName,            // ← NEW
                salesRepo,
                productService
            );

            if (invoice != null)
            {
                // Lab 11: Update customer total spent
                if (customerId.HasValue)
                {
                    var customerService = new CustomerService(new CustomerDAL());
                    customerService.UpdateTotalSpent(customerId.Value, invoice.Total);
                }

                Console.WriteLine("\n  ✅ TRANSACTION COMPLETE!");
                AdminMenu.PrintInvoice(invoice);
            }
        }
        // ── Cash Payment ──
        private bool ProcessCashPayment()
        {
            decimal total = shoppingCartService.GetTotal();

            Console.WriteLine($"\n  ── Cash Payment ──");
            Console.WriteLine($"  Total Due: {total:F2}");
            Console.Write("  Amount Received: ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal received))
            {
                Console.WriteLine("  [Error] Invalid amount!");
                return false;
            }

            if (received < total)
            {
                Console.WriteLine($"  [Error] Insufficient amount! Need {total:F2}, received {received:F2}");
                return false;
            }

            decimal change = received - total;
            Console.WriteLine($"  Amount Received: {received:F2}");
            Console.WriteLine($"  Change Due:      {change:F2}");
            Console.WriteLine("   Cash payment accepted!");
            return true;
        }

        // ── Credit Card Payment with full validation ──
        private bool ProcessCreditCardPayment()
        {
            decimal total = shoppingCartService.GetTotal();

            Console.WriteLine($"\n  ── Credit Card Payment ──");
            Console.WriteLine($"  Total to charge: {total:F2}");

            Console.Write("  Card Number (16 digits): ");
            string cardNumber = Console.ReadLine() ?? "";

            Console.Write("  Expiration Date (MM/YY): ");
            string expiryDate = Console.ReadLine() ?? "";

            Console.Write("  CVV (3 digits): ");
            string cvv = Console.ReadLine() ?? "";

            var payment = new Payment
            {
                Amount = total,
                PaymentMethod = "Credit Card",
                CardNumber = cardNumber,
                ExpirationDate = expiryDate,
                CVV = cvv
            };

            var gateway = new CreditCardPaymentGateway();
            var paymentService = new PaymentService(gateway);
            return paymentService.ProcessPayment(payment);
        }

        // ── PayPal Payment with email validation ──
        private bool ProcessPayPalPayment()
        {
            decimal total = shoppingCartService.GetTotal();

            Console.WriteLine($"\n  ── PayPal Payment ──");
            Console.WriteLine($"  Total to charge: {total:F2}");

            Console.Write("  PayPal Email: ");
            string email = Console.ReadLine() ?? "";

            var payment = new Payment
            {
                Amount = total,
                PaymentMethod = "PayPal",
                PayPalEmail = email
            };

            var gateway = new PayPalPaymentGateway();
            var paymentService = new PaymentService(gateway);
            return paymentService.ProcessPayment(payment);
        }

        private void CancelTransaction()
        {
            if (shoppingCartService.IsCartEmpty())
            {
                Console.WriteLine("  Cart is already empty.");
                return;
            }

            Console.Write("  Are you sure you want to cancel? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                shoppingCartService.CancelTransaction();
            }
        }

        private void NewTransaction()
        {
            if (!shoppingCartService.IsCartEmpty())
            {
                Console.Write("  Current cart will be cleared. Continue? (y/n): ");
                if (Console.ReadLine()?.ToLower() != "y") return;
            }
            shoppingCartService.NewTransaction();
        }

        private void ViewPastInvoices()
        {
            var salesService = new SalesService(salesRepo, productService);
            var invoices = salesService.GetAllInvoices();

            if (invoices.Count == 0)
            { Console.WriteLine("  No past invoices."); return; }

            Console.WriteLine($"\n  {"ID",-6} {"Invoice #",-22} {"Date",-20} {"Paid Via",-14} {"Total",-10}");
            Console.WriteLine("  " + new string('-', 74));
            foreach (var inv in invoices)
            {
                Console.WriteLine($"  {inv.InvoiceId,-6} {inv.InvoiceNumber,-22} {inv.Date:yyyy-MM-dd HH:mm}  {inv.PaymentMethod,-14} {inv.Total,-10:F2}");
            }

            Console.Write("\n  View details? Enter Invoice ID (0 to skip): ");
            if (int.TryParse(Console.ReadLine(), out int id) && id > 0)
            {
                var invoice = salesService.GetInvoiceById(id);
                if (invoice != null)
                    AdminMenu.PrintInvoice(invoice);
                else
                    Console.WriteLine("  Invoice not found.");
            }
        }
    }
}