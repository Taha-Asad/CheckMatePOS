using System.Collections.Generic;
using CheckMatePOS.Models;

namespace CheckMatePOS.Interfaces
{
    public interface ISalesRepository
    {
        int SaveInvoice(Invoice invoice);
        void SaveInvoiceItems(int invoiceId, List<InvoiceItem> items);
        List<Invoice> GetAllInvoices();
        Invoice? GetInvoiceById(int invoiceId);
        string GenerateInvoiceNumber();
    }
}