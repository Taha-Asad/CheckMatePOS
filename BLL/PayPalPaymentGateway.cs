using System;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class PayPalPaymentGateway : IPaymentGateway
    {
        public bool ProcessPayment(Payment payment)
        {
            Console.WriteLine("\n  ── PayPal Payment Processing ──");

            // Validate PayPal email
            if (string.IsNullOrWhiteSpace(payment.PayPalEmail))
            {
                Console.WriteLine("  [Error] PayPal email cannot be empty.");
                return false;
            }

            string email = payment.PayPalEmail.Trim();

            // Basic email validation
            if (!email.Contains('@'))
            {
                Console.WriteLine("  [Error] Invalid email: must contain '@'.");
                return false;
            }

            string[] emailParts = email.Split('@');

            if (emailParts.Length != 2)
            {
                Console.WriteLine("  [Error] Invalid email format.");
                return false;
            }

            string localPart = emailParts[0];
            string domainPart = emailParts[1];

            if (string.IsNullOrWhiteSpace(localPart))
            {
                Console.WriteLine("  [Error] Invalid email: username before '@' is empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(domainPart) || !domainPart.Contains('.'))
            {
                Console.WriteLine("  [Error] Invalid email: domain must contain '.' (e.g., gmail.com).");
                return false;
            }

            // Check domain has text before and after dot
            string[] domainSplit = domainPart.Split('.');
            if (domainSplit.Length < 2)
            {
                Console.WriteLine("  [Error] Invalid email domain.");
                return false;
            }

            foreach (var part in domainSplit)
            {
                if (string.IsNullOrWhiteSpace(part))
                {
                    Console.WriteLine("  [Error] Invalid email domain format.");
                    return false;
                }
            }

            // Validate amount
            if (payment.Amount <= 0)
            {
                Console.WriteLine("  [Error] Payment amount must be greater than 0.");
                return false;
            }

            Console.WriteLine($"  PayPal:  {email}");
            Console.WriteLine($"  Amount:  {payment.Amount:F2}");
            Console.WriteLine($"  Processing PayPal payment of {payment.Amount:F2}...");
            Console.WriteLine("  PayPal payment successful!");

            return true;
        }
    }
}