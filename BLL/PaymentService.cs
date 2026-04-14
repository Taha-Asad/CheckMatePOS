using System;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class PaymentService
    {
        private readonly IPaymentGateway paymentGateway;

        public PaymentService(IPaymentGateway gateway)
        {
            paymentGateway = gateway;
        }

        public bool ProcessPayment(Payment payment)
        {
            if (paymentGateway.ProcessPayment(payment))
            {
                Console.WriteLine("  Payment processed successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("  Payment failed.");
                return false;
            }
        }
    }
}