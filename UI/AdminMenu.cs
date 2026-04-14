using System;
using CheckMatePOS.BLL;
using CheckMatePOS.Models;

namespace CheckMatePOS.UI
{
    public class AdminMenu
    {
        private readonly AuthService authService;
        private readonly ProductService productService;
        private readonly SalesService salesService;
        private readonly CustomerService customerService;
        private readonly ReportingService reportingService;
        public AdminMenu(AuthService auth, ProductService product, SalesService sales, CustomerService customers, ReportingService reports)
        {
            authService = auth;
            productService = product;
            salesService = sales;
            customerService = customers;
            reportingService = reports;
        }

        public void Show()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("          ADMIN DASHBOARD              ");
                Console.WriteLine("  1.  Add Product                       ");
                Console.WriteLine("  2.  View All Products                 ");
                Console.WriteLine("  3.  Update Product                    ");
                Console.WriteLine("  4.  Delete Product                    ");
                Console.WriteLine("  5.  Add Customer                      ");  // ← NEW
                Console.WriteLine("  6.  View All Customers                ");  // ← NEW
                Console.WriteLine("  7.  Update Customer                   ");  // ← NEW
                Console.WriteLine("  8.  Delete Customer                   ");  // ← NEW
                Console.WriteLine("  9.  Create Cashier Account            ");
                Console.WriteLine("  10. View All Users                   ");
                Console.WriteLine("  11. Activate/Deactivate User         ");
                Console.WriteLine("  12. Sales Report                     ");  // ← NEW
                Console.WriteLine("  13. Customer Spending Report         ");  // ← NEW
                Console.WriteLine("  14. Product Sales Report             ");  // ← NEW
                Console.WriteLine("  15. Inventory Status Report          ");  // ← NEW
                Console.WriteLine("  16. View All Invoices                ");
                Console.WriteLine("  17. View Invoice Details             ");
                Console.WriteLine("                                       ");
                Console.WriteLine("  0. Logout                            ");

                Console.Write("  Choice: ");

                switch (Console.ReadLine())
                {
                    case "1": AddProduct(); break;
                    case "2": ViewAllProducts(); break;
                    case "3": UpdateProduct(); break;
                    case "4": DeleteProduct(); break;
                    case "5": AddCustomer(); break;        // ← NEW
                    case "6": ViewAllCustomers(); break;   // ← NEW
                    case "7": UpdateCustomer(); break;     // ← NEW
                    case "8": DeleteCustomer(); break;     // ← NEW
                    case "9": CreateCashier(); break;
                    case "10": ViewAllUsers(); break;
                    case "11": ToggleUser(); break;
                    case "12": reportingService.GenerateSalesReport(); break;          // ← NEW
                    case "13": reportingService.GenerateCustomerReport(); break;       // ← NEW
                    case "14": reportingService.GenerateProductReport(); break;        // ← NEW
                    case "15": reportingService.GenerateInventoryReport(); break;      // ← NEW
                    case "16": ViewAllInvoices(); break;
                    case "17": ViewInvoiceDetails(); break;
                    case "0":
                        authService.Logout();
                        running = false;
                        break;
                    default:
                        Console.WriteLine("  Invalid choice!");
                        break;
                }
            }
        }

        private void AddProduct()
        {
            Console.WriteLine("\n  --- Add Product ---");
            Console.Write("  Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("  Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            { Console.WriteLine("  Invalid price!"); return; }

            Console.Write("  Stock Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty))
            { Console.WriteLine("  Invalid quantity!"); return; }

            productService.AddProduct(name, price, qty);
        }

        private void ViewAllProducts()
        {
            var products = productService.GetAllProducts();
            if (products.Count == 0)
            { Console.WriteLine("  No products found."); return; }

            Console.WriteLine($"\n  {"ID",-6} {"Name",-20} {"Price",-12} {"Stock",-8}");
            Console.WriteLine("  " + new string('-', 48));
            foreach (var p in products)
            {
                Console.WriteLine($"  {p.ProductId,-6} {p.ProductName,-20} {p.Price,-12:F2} {p.QuantityInStock,-8}");
            }
        }

        private void UpdateProduct()
        {
            ViewAllProducts();
            Console.Write("\n  Enter Product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            var product = productService.GetProductById(id);
            if (product == null)
            { Console.WriteLine("  Product not found!"); return; }

            Console.Write($"  New Name [{product.ProductName}]: ");
            string name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name)) name = product.ProductName;

            Console.Write($"  New Price [{product.Price}]: ");
            string priceStr = Console.ReadLine() ?? "";
            decimal price = string.IsNullOrWhiteSpace(priceStr) ? product.Price : decimal.Parse(priceStr);

            Console.Write($"  New Stock [{product.QuantityInStock}]: ");
            string qtyStr = Console.ReadLine() ?? "";
            int qty = string.IsNullOrWhiteSpace(qtyStr) ? product.QuantityInStock : int.Parse(qtyStr);

            productService.UpdateProduct(id, name, price, qty);
        }

        private void DeleteProduct()
        {
            ViewAllProducts();
            Console.Write("\n  Enter Product ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            Console.Write("  Are you sure? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
                productService.DeleteProduct(id);
        }

        private void CreateCashier()
        {
            Console.WriteLine("\n  --- Create Cashier Account ---");
            Console.Write("  Username: ");
            string username = Console.ReadLine() ?? "";
            Console.Write("  Password: ");
            string password = Console.ReadLine() ?? "";

            authService.CreateCashier(username, password);
        }

        private void ViewAllUsers()
        {
            var users = authService.GetAllUsers();

            Console.WriteLine($"\n  {"ID",-6} {"Username",-15} {"Role",-10} {"Status",-10}");
            Console.WriteLine("  " + new string('-', 43));
            foreach (var u in users)
            {
                string status = u.IsActive ? "Active" : "Inactive";
                Console.WriteLine($"  {u.UserId,-6} {u.Username,-15} {u.Role,-10} {status,-10}");
            }
        }

        private void ToggleUser()
        {
            ViewAllUsers();
            Console.Write("\n  Enter User ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            Console.Write("  Activate (a) or Deactivate (d): ");
            string choice = Console.ReadLine()?.ToLower() ?? "";

            if (choice == "a")
                authService.ToggleUserStatus(id, true);
            else if (choice == "d")
                authService.ToggleUserStatus(id, false);
            else
                Console.WriteLine("  Invalid choice!");
        }

        private void AddCustomer()
        {
            Console.WriteLine("\n  --- Add Customer ---");
            Console.Write("  Name: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("  Email: ");
            string email = Console.ReadLine() ?? "";

            Console.Write("  Phone: ");
            string phone = Console.ReadLine() ?? "";

            customerService.AddCustomer(name, email, phone);
        }

        private void ViewAllCustomers()
        {
            var customers = customerService.GetAllCustomers();
            if (customers.Count == 0)
            { Console.WriteLine("  No customers found."); return; }

            Console.WriteLine($"\n  {"ID",-6} {"Name",-20} {"Email",-25} {"Phone",-15} {"Total Spent",-12}");
            Console.WriteLine("  " + new string('-', 80));
            foreach (var c in customers)
            {
                Console.WriteLine($"  {c.CustomerId,-6} {c.Name,-20} {c.Email,-25} {c.Phone,-15} {c.TotalSpent,-12:F2}");
            }
        }

        private void UpdateCustomer()
        {
            ViewAllCustomers();
            Console.Write("\n  Enter Customer ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            var customer = customerService.GetCustomerById(id);
            if (customer == null)
            { Console.WriteLine("  Customer not found!"); return; }

            Console.Write($"  New Name [{customer.Name}]: ");
            string name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name)) name = customer.Name;

            Console.Write($"  New Email [{customer.Email}]: ");
            string email = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(email)) email = customer.Email;

            Console.Write($"  New Phone [{customer.Phone}]: ");
            string phone = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(phone)) phone = customer.Phone;

            customerService.UpdateCustomer(id, name, email, phone);
        }

        private void DeleteCustomer()
        {
            ViewAllCustomers();
            Console.Write("\n  Enter Customer ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            Console.Write("  Are you sure? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
                customerService.DeleteCustomer(id);
        }
        private void ViewAllInvoices()
        {
            var invoices = salesService.GetAllInvoices();
            if (invoices.Count == 0)
            { Console.WriteLine("  No invoices found."); return; }

            Console.WriteLine($"\n  {"ID",-6} {"Invoice #",-22} {"Date",-20} {"Cashier",-12} {"Paid Via",-14} {"Total",-10}");
            Console.WriteLine("  " + new string('-', 86));
            foreach (var inv in invoices)
            {
                Console.WriteLine($"  {inv.InvoiceId,-6} {inv.InvoiceNumber,-22} {inv.Date:yyyy-MM-dd HH:mm}  {inv.CashierName,-12} {inv.PaymentMethod,-14} {inv.Total,-10:F2}");
            }
        }

        private void ViewInvoiceDetails()
        {
            Console.Write("\n  Enter Invoice ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { Console.WriteLine("  Invalid ID!"); return; }

            var invoice = salesService.GetInvoiceById(id);
            if (invoice == null)
            { Console.WriteLine("  Invoice not found!"); return; }

            PrintInvoice(invoice);
        }

        // Lab 10: Updated PrintInvoice to show PaymentMethod
        // Lab 11: Updated PrintInvoice to show customer
        public static void PrintInvoice(Invoice invoice)
        {
            Console.WriteLine("\n  ╔══════════════════════════════════════════╗");
            Console.WriteLine("  ║            CheckMate POS                ║");
            Console.WriteLine("  ║              INVOICE                    ║");
            Console.WriteLine("  ╠══════════════════════════════════════════╣");
            Console.WriteLine($"  ║  Invoice #: {invoice.InvoiceNumber,-28} ║");
            Console.WriteLine($"  ║  Date:      {invoice.Date:yyyy-MM-dd HH:mm,-28} ║");
            Console.WriteLine($"  ║  Cashier:   {invoice.CashierName,-28} ║");
            Console.WriteLine($"  ║  Customer:  {invoice.CustomerName,-28} ║");  // ← NEW
            Console.WriteLine($"  ║  Paid Via:  {invoice.PaymentMethod,-28} ║");
            Console.WriteLine("  ╠══════════════════════════════════════════╣");
            Console.WriteLine($"  ║  {"Item",-18} {"Qty",-5} {"Price",-8} {"Total",-8} ║");
            Console.WriteLine("  ║──────────────────────────────────────────║");

            foreach (var item in invoice.Items)
            {
                Console.WriteLine($"  ║  {item.ProductName,-18} {item.Quantity,-5} {item.UnitPrice,-8:F2} {item.LineTotal,-8:F2} ║");
            }

            Console.WriteLine("  ╠══════════════════════════════════════════╣");
            Console.WriteLine($"  ║  Subtotal:  {invoice.Subtotal,28:F2} ║");
            Console.WriteLine($"  ║  Tax (17%): {invoice.TaxAmount,28:F2} ║");
            Console.WriteLine($"  ║  TOTAL:     {invoice.Total,28:F2} ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
        }
    }
}