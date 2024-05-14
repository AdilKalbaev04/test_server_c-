using CSharpCornerApi.Data;
using CSharpCornerApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
   public interface IBillingService 
{
    Task<Invoice> CreateInvoice(Invoice invoice);
    Task<Transaction> ProcessPayment(Transaction transaction);
        Task<List<Invoice>> GetInvoicesForUser(string userId);

}

public class BillingService : IBillingService 
{
   private readonly AppDbContext _context;

   public BillingService(AppDbContext context)
   {
       _context = context;
   }

public async Task<Invoice> CreateInvoice(Invoice invoice)
{
   invoice.CreatedDate = invoice.CreatedDate.ToUniversalTime();
   _context.Invoices.Add(invoice);
   await _context.SaveChangesAsync();
   return invoice;
}

public async Task<Transaction> ProcessPayment(Transaction transaction)
{
   var invoice = await _context.Invoices.FindAsync(transaction.InvoiceId);
   
   if (invoice == null)
   {
       throw new Exception("Invoice not found");
   }

   invoice.Amount -= transaction.AmountPaid;
   transaction.TransactionDate = transaction.TransactionDate.ToUniversalTime();
   _context.Transactions.Add(transaction);
   await _context.SaveChangesAsync();
   return transaction;
}

public async Task<List<Invoice>> GetInvoicesForUser(string userId)
   {
       return await _context.Invoices.Where(invoice => invoice.UserId == userId).ToListAsync();
   }

}
