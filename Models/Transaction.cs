namespace CSharpCornerApi.Models
{
public class Transaction
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public decimal AmountPaid { get; set;}
    public DateTime TransactionDate { get;set;}
}
}