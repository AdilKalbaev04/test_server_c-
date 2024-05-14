using CSharpCornerApi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IEmailService _emailService;

    public NotificationsController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("sendEmail")]
    public async Task<IActionResult> SendEmail([FromBody] EmailModel model)
    {
        await _emailService.SendEmailAsync(model.Email, model.Subject, model.Message);
        return Ok();
    }
}
