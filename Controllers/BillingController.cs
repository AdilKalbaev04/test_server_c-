using CSharpCornerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase 
{
  private readonly IBillingService _billingService;
  private readonly ILogger<BillingController> _logger;

  public BillingController(IBillingService billingService, ILogger<BillingController> logger)
  {
    _billingService = billingService;
    _logger = logger;
  }

 [HttpPost("CreateInvoice")]
public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice) 
{
    _logger.LogInformation("Creating invoice with description {Description} and amount {Amount}", invoice.Description, invoice.Amount);
    var result = await _billingService.CreateInvoice(invoice);
    _logger.LogInformation("Invoice created with ID {Id}", result.Id);
    return Ok(result);
}

[HttpPost("ProcessPayment")]
public async Task<IActionResult> ProcessPayment([FromBody] Transaction transaction) 
{
    _logger.LogInformation("Processing payment for invoice ID {InvoiceId} with amount {Amount}", transaction.InvoiceId, transaction.AmountPaid);
    var result = await _billingService.ProcessPayment(transaction);
    _logger.LogInformation("Payment processed with transaction ID {Id}", result.Id);
    return Ok(result);
}

[HttpGet("GetInvoices/{userId}")]
public async Task<IActionResult> GetInvoices(string userId) 
{
    var invoices = await _billingService.GetInvoicesForUser(userId);
    return Ok(invoices);
}

}
