using CSharpCornerApi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase 
{
  private readonly IBillingService _billingService;

  public BillingController(IBillingService billingService)
  {
    _billingService = billingService;
  }

  [HttpPost("CreateInvoice")]
  public async Task<IActionResult> CreateInvoice([FromBody] Invoice invoice) 
  {
     var result = await _billingService.CreateInvoice(invoice);
     return Ok(result);
  }

  [HttpPost("ProcessPayment")]
  public async Task<IActionResult> ProcessPayment([FromBody] Transaction transaction) 
  {
     var result = await _billingService.ProcessPayment(transaction);
     return Ok(result);
  }
}
