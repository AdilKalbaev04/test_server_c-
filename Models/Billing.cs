using System;

namespace CSharpCornerApi.Models
{

   public interface IBillingService 
{
    Task<Invoice> CreateInvoice(Invoice invoice);
    Task<Transaction> ProcessPayment(Transaction transaction);
}
public class Invoice
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDate { get; set; }
}
public class Transaction
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public decimal AmountPaid { get; set;}
    public DateTime TransactionDate { get;set;}
}

public class Payment
{
    public int Id {get;set;}
    public int TransactionId{get;set;}
    public string PaymentMethod{get;set;} 
}

}


