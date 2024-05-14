using System;

namespace CSharpCornerApi.Models
{
public class Invoice
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string UserId { get; set; }
}
}
