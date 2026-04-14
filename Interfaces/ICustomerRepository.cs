using System.Collections.Generic;
using CheckMatePOS.Models;

namespace CheckMatePOS.Interfaces
{
    public interface ICustomerRepository
    {
        void AddCustomer(Customer customer);
        void UpdateCustomer(int customerId, Customer updatedCustomer);
        void DeleteCustomer(int customerId);
        List<Customer> GetAllCustomers();
        Customer? GetCustomerById(int customerId);
        Customer? GetCustomerByEmail(string email);
        bool EmailExists(string email);
        void UpdateTotalSpent(int customerId, decimal amount);
    }
}