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
   private readonly IEmailService _emailService; 

   public BillingService(AppDbContext context, IEmailService emailService) 
   {
       _context = context;
       _emailService = emailService; 
   }
public async Task<Invoice> CreateInvoice(Invoice invoice)
{
   invoice.CreatedDate = invoice.CreatedDate.ToUniversalTime();
   _context.Invoices.Add(invoice);
   await _context.SaveChangesAsync();
     await _emailService.SendEmailAsync(invoice.UserId, "New Invoice Created", $"A new invoice with ID {invoice.Id} has been created.");
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
