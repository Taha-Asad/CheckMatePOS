using System;
using System.Collections.Generic;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class CustomerService
    {
        private readonly ICustomerRepository customerRepo;

        public CustomerService(ICustomerRepository repository)
        {
            customerRepo = repository;
        }

        public bool AddCustomer(string name, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("  [Error] Customer name cannot be empty!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("  [Error] Email cannot be empty!");
                return false;
            }

            if (!email.Contains('@'))
            {
                Console.WriteLine("  [Error] Invalid email format!");
                return false;
            }

            if (customerRepo.EmailExists(email))
            {
                Console.WriteLine("  [Error] Email already exists!");
                return false;
            }

            var customer = new Customer
            {
                Name = name,
                Email = email,
                Phone = phone,
                TotalSpent = 0
            };

            customerRepo.AddCustomer(customer);
            Console.WriteLine($"  Customer '{name}' added! (ID: {customer.CustomerId})");
            return true;
        }

        public bool UpdateCustomer(int id, string name, string email, string phone)
        {
            var existing = customerRepo.GetCustomerById(id);
            if (existing == null)
            {
                Console.WriteLine("  [Error] Customer not found!");
                return false;
            }

            if (string.IsNullOrWhiteSpace(name)) name = existing.Name;
            if (string.IsNullOrWhiteSpace(email)) email = existing.Email;
            if (string.IsNullOrWhiteSpace(phone)) phone = existing.Phone;

            var updated = new Customer
            {
                Name = name,
                Email = email,
                Phone = phone,
                TotalSpent = existing.TotalSpent
            };

            customerRepo.UpdateCustomer(id, updated);
            Console.WriteLine($"  Customer ID {id} updated!");
            return true;
        }

        public bool DeleteCustomer(int id)
        {
            var customer = customerRepo.GetCustomerById(id);
            if (customer == null)
            {
                Console.WriteLine("  [Error] Customer not found!");
                return false;
            }

            customerRepo.DeleteCustomer(id);
            Console.WriteLine($"  Customer '{customer.Name}' deleted!");
            return true;
        }

        public List<Customer> GetAllCustomers() => customerRepo.GetAllCustomers();

        public Customer? GetCustomerById(int id) => customerRepo.GetCustomerById(id);

        public Customer? GetCustomerByEmail(string email) => customerRepo.GetCustomerByEmail(email);

        public void UpdateTotalSpent(int customerId, decimal amount)
        {
            customerRepo.UpdateTotalSpent(customerId, amount);
        }
    }
}