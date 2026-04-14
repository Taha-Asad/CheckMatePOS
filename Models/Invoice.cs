using System;
using System.Collections.Generic;

namespace CheckMatePOS.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int CashierId { get; set; }
        public string CashierName { get; set; } = string.Empty;

        // Lab 11: Added customer tracking
        public int? CustomerId { get; set; }                    // ← NEW
        public string CustomerName { get; set; } = "Walk-in";   // ← NEW

        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
}