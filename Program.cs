using System;
using CheckMatePOS.DAL;
using CheckMatePOS.BLL;
using CheckMatePOS.UI;
using CheckMatePOS.Interfaces;

namespace CheckMatePOS
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseHelper.InitializeDatabase();

            // DAL instances
            IUserRepository userDAL = new UserDAL();
            IProductRepository productDAL = new ProductDAL();
            ISalesRepository salesDAL = new SalesDAL();
            ICartRepository cartDAL = new CartDAL();
            ICustomerRepository customerDAL = new CustomerDAL();    // ← NEW

            // BLL instances
            AuthService authService = new AuthService(userDAL);
            ProductService productService = new ProductService(productDAL);
            SalesService salesService = new SalesService(salesDAL, productService);
            ShoppingCartService shoppingCartService = new ShoppingCartService(productDAL, cartDAL);
            CustomerService customerService = new CustomerService(customerDAL);                      // ← NEW
            ReportingService reportingService = new ReportingService(salesDAL, customerDAL, productDAL);  // ← NEW

            // UI instances
            LoginScreen loginScreen = new LoginScreen(authService);
            AdminMenu adminMenu = new AdminMenu(authService, productService, salesService, customerService, reportingService);  // ← UPDATED
            CashierMenu cashierMenu = new CashierMenu(authService, productService, shoppingCartService, salesDAL);

            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║                                          ║");
            Console.WriteLine("║          CheckMate POS System            ║");
            Console.WriteLine("║          Point of Sale v1.0              ║");
            Console.WriteLine("║                                          ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.WriteLine("  Default Admin → username: admin | password: admin123\n");

            bool appRunning = true;
            while (appRunning)
            {
                bool loggedIn = loginScreen.Show();

                if (!loggedIn)
                {
                    Console.Write("\n  Try again? (y/n): ");
                    if (Console.ReadLine()?.ToLower() != "y")
                        appRunning = false;
                    continue;
                }

                if (authService.IsAdmin())
                {
                    adminMenu.Show();
                }
                else if (authService.IsCashier())
                {
                    cashierMenu.Show();
                }

                Console.Write("\n  Another user login? (y/n): ");
                if (Console.ReadLine()?.ToLower() != "y")
                    appRunning = false;
            }

            Console.WriteLine("\n  CheckMate POS shutting down. Goodbye!");
        }
    }
}