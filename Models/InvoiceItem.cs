namespace CheckMatePOS.Models
{
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }
}