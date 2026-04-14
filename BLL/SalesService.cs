using System.Collections.Generic;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class SalesService
    {
        private readonly ISalesRepository salesRepo;
        private readonly ProductService productService;

        public SalesService(ISalesRepository salesRepository, ProductService prodService)
        {
            salesRepo = salesRepository;
            productService = prodService;
        }

        public List<Invoice> GetAllInvoices()
        {
            return salesRepo.GetAllInvoices();
        }

        public Invoice? GetInvoiceById(int id)
        {
            return salesRepo.GetInvoiceById(id);
        }
    }
}