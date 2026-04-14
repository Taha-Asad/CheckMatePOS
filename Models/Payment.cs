namespace CheckMatePOS.Models
{
    public class Payment
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;   // "Credit Card", "PayPal", "Cash"
        public string CardNumber { get; set; } = string.Empty;      // For Credit Card
        public string ExpirationDate { get; set; } = string.Empty;  // For Credit Card (MM/YY)
        public string CVV { get; set; } = string.Empty;             // For Credit Card
        public string PayPalEmail { get; set; } = string.Empty;     // For PayPal
    }
}