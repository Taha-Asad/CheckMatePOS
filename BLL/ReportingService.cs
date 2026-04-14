using System;
using System.Collections.Generic;
using System.Linq;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class ReportingService
    {
        private readonly ISalesRepository salesRepo;
        private readonly ICustomerRepository customerRepo;
        private readonly IProductRepository productRepo;

        public ReportingService(ISalesRepository sales, ICustomerRepository customer, IProductRepository product)
        {
            salesRepo = sales;
            customerRepo = customer;
            productRepo = product;
        }

        // Lab 11: Generate Sales Report (total sales from all invoices)
        public void GenerateSalesReport()
        {
            var invoices = salesRepo.GetAllInvoices();

            if (invoices.Count == 0)
            {
                Console.WriteLine("  No sales data available.");
                return;
            }

            decimal totalSales = invoices.Sum(i => i.Total);
            decimal totalTax = invoices.Sum(i => i.TaxAmount);
            decimal totalSubtotal = invoices.Sum(i => i.Subtotal);
            int totalTransactions = invoices.Count;

            Console.WriteLine("\n  ╔════════════════════════════════════════╗");
            Console.WriteLine("  ║           SALES REPORT                ║");
            Console.WriteLine("  ╠════════════════════════════════════════╣");
            Console.WriteLine($"  ║  Total Transactions: {totalTransactions,-17} ║");
            Console.WriteLine($"  ║  Subtotal:           {totalSubtotal,-17:F2} ║");
            Console.WriteLine($"  ║  Tax Collected:      {totalTax,-17:F2} ║");
            Console.WriteLine($"  ║  TOTAL SALES:        {totalSales,-17:F2} ║");
            Console.WriteLine("  ╚════════════════════════════════════════╝");

            // Sales by payment method
            var byMethod = invoices.GroupBy(i => i.PaymentMethod)
                                   .Select(g => new { Method = g.Key, Total = g.Sum(i => i.Total) })
                                   .OrderByDescending(x => x.Total);

            Console.WriteLine("\n  Sales by Payment Method:");
            Console.WriteLine($"  {"Method",-15} {"Total",-15}");
            Console.WriteLine("  " + new string('-', 32));
            foreach (var item in byMethod)
            {
                Console.WriteLine($"  {item.Method,-15} {item.Total,-15:F2}");
            }
        }

        // Lab 11: Generate Customer Report (spending per customer)
        public void GenerateCustomerReport()
        {
            var customers = customerRepo.GetAllCustomers();

            if (customers.Count == 0)
            {
                Console.WriteLine("  No customers found.");
                return;
            }

            Console.WriteLine("\n  ╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║              CUSTOMER SPENDING REPORT                  ║");
            Console.WriteLine("  ╠════════════════════════════════════════════════════════╣");
            Console.WriteLine($"  ║  {"Customer",-25} {"Email",-18} {"Total Spent",-10} ║");
            Console.WriteLine("  ║────────────────────────────────────────────────────────║");

            var sortedCustomers = customers.OrderByDescending(c => c.TotalSpent);

            foreach (var customer in sortedCustomers)
            {
                Console.WriteLine($"  ║  {customer.Name,-25} {customer.Email,-18} {customer.TotalSpent,-10:F2} ║");
            }

            decimal totalCustomerSpending = customers.Sum(c => c.TotalSpent);
            Console.WriteLine("  ╠════════════════════════════════════════════════════════╣");
            Console.WriteLine($"  ║  TOTAL:                                    {totalCustomerSpending,-10:F2} ║");
            Console.WriteLine("  ╚════════════════════════════════════════════════════════╝");
        }

        // Bonus: Top selling products
        public void GenerateProductReport()
        {
            var invoices = salesRepo.GetAllInvoices();
            var products = productRepo.GetAllProducts();

            if (invoices.Count == 0)
            {
                Console.WriteLine("  No sales data available.");
                return;
            }

            // Aggregate all invoice items
            var productSales = new Dictionary<int, (string Name, int QtySold, decimal Revenue)>();

            foreach (var invoice in invoices)
            {
                foreach (var item in invoice.Items)
                {
                    if (!productSales.ContainsKey(item.ProductId))
                    {
                        productSales[item.ProductId] = (item.ProductName, 0, 0);
                    }

                    var current = productSales[item.ProductId];
                    productSales[item.ProductId] = (current.Name, current.QtySold + item.Quantity, current.Revenue + item.LineTotal);
                }
            }

            var sorted = productSales.OrderByDescending(x => x.Value.Revenue);

            Console.WriteLine("\n  ╔════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║            PRODUCT SALES REPORT                   ║");
            Console.WriteLine("  ╠════════════════════════════════════════════════════╣");
            Console.WriteLine($"  ║  {"Product",-25} {"Qty Sold",-12} {"Revenue",-12} ║");
            Console.WriteLine("  ║────────────────────────────────────────────────────║");

            foreach (var item in sorted)
            {
                Console.WriteLine($"  ║  {item.Value.Name,-25} {item.Value.QtySold,-12} {item.Value.Revenue,-12:F2} ║");
            }

            Console.WriteLine("  ╚════════════════════════════════════════════════════╝");
        }

        // Inventory status report
        public void GenerateInventoryReport()
        {
            var products = productRepo.GetAllProducts();

            if (products.Count == 0)
            {
                Console.WriteLine("  No products in inventory.");
                return;
            }

            var lowStock = products.Where(p => p.QuantityInStock < 10).ToList();
            var outOfStock = products.Where(p => p.QuantityInStock == 0).ToList();

            Console.WriteLine("\n  ╔════════════════════════════════════════════════════╗");
            Console.WriteLine("  ║            INVENTORY STATUS REPORT                ║");
            Console.WriteLine("  ╠════════════════════════════════════════════════════╣");
            Console.WriteLine($"  ║  Total Products:        {products.Count,-27} ║");
            Console.WriteLine($"  ║  Low Stock (<10):       {lowStock.Count,-27} ║");
            Console.WriteLine($"  ║  Out of Stock:          {outOfStock.Count,-27} ║");
            Console.WriteLine("  ╚════════════════════════════════════════════════════╝");

            if (lowStock.Count > 0)
            {
                Console.WriteLine("\n  Low Stock Alert:");
                Console.WriteLine($"  {"Product",-25} {"Current Stock",-15}");
                Console.WriteLine("  " + new string('-', 42));
                foreach (var p in lowStock)
                {
                    Console.WriteLine($"  {p.ProductName,-25} {p.QuantityInStock,-15}");
                }
            }

            if (outOfStock.Count > 0)
            {
                Console.WriteLine("\n  ⚠️  OUT OF STOCK:");
                foreach (var p in outOfStock)
                {
                    Console.WriteLine($"  - {p.ProductName}");
                }
            }
        }
    }
}