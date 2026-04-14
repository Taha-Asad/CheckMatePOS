using CheckMatePOS.Models;

namespace CheckMatePOS.Interfaces
{
    public interface IPaymentGateway
    {
        bool ProcessPayment(Payment payment);
    }
}