using System;
using System.Globalization;
using System.Linq;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class CreditCardPaymentGateway : IPaymentGateway
    {
        public bool ProcessPayment(Payment payment)
        {
            Console.WriteLine("\n  ── Credit Card Payment Processing ──");

            // Validate card number: must be exactly 16 digits
            if (string.IsNullOrWhiteSpace(payment.CardNumber))
            {
                Console.WriteLine("  [Error] Card number cannot be empty.");
                return false;
            }

            string cardDigits = payment.CardNumber.Replace(" ", "").Replace("-", "");

            if (cardDigits.Length != 16)
            {
                Console.WriteLine("  [Error] Card number must be exactly 16 digits.");
                return false;
            }

            if (!cardDigits.All(char.IsDigit))
            {
                Console.WriteLine("  [Error] Card number must contain only digits.");
                return false;
            }

            // Validate expiration date: format MM/YY and not expired
            if (string.IsNullOrWhiteSpace(payment.ExpirationDate))
            {
                Console.WriteLine("  [Error] Expiration date cannot be empty.");
                return false;
            }

            if (!payment.ExpirationDate.Contains('/') || payment.ExpirationDate.Length != 5)
            {
                Console.WriteLine("  [Error] Expiration date must be in MM/YY format.");
                return false;
            }

            string[] parts = payment.ExpirationDate.Split('/');
            if (parts.Length != 2)
            {
                Console.WriteLine("  [Error] Expiration date must be in MM/YY format.");
                return false;
            }

            if (!int.TryParse(parts[0], out int month) || !int.TryParse(parts[1], out int year))
            {
                Console.WriteLine("  [Error] Expiration date must contain valid numbers.");
                return false;
            }

            if (month < 1 || month > 12)
            {
                Console.WriteLine("  [Error] Month must be between 01 and 12.");
                return false;
            }

            // Convert 2-digit year to 4-digit
            int fullYear = 2000 + year;
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            if (fullYear < currentYear || (fullYear == currentYear && month < currentMonth))
            {
                Console.WriteLine("  [Error] Card has expired!");
                return false;
            }

            // Validate CVV: must be exactly 3 digits
            if (string.IsNullOrWhiteSpace(payment.CVV))
            {
                Console.WriteLine("  [Error] CVV cannot be empty.");
                return false;
            }

            if (payment.CVV.Length != 3 || !payment.CVV.All(char.IsDigit))
            {
                Console.WriteLine("  [Error] CVV must be exactly 3 digits.");
                return false;
            }

            // Validate amount
            if (payment.Amount <= 0)
            {
                Console.WriteLine("  [Error] Payment amount must be greater than 0.");
                return false;
            }

            // Mask card number for display (show last 4 digits)
            string maskedCard = new string('*', 12) + cardDigits.Substring(12);

            Console.WriteLine($"  Card:    {maskedCard}");
            Console.WriteLine($"  Expiry:  {payment.ExpirationDate}");
            Console.WriteLine($"  Amount:  {payment.Amount:F2}");
            Console.WriteLine($"  Processing Credit Card payment of {payment.Amount:F2}...");
            Console.WriteLine("   Credit Card payment successful!");

            return true;
        }
    }
}