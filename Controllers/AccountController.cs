using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CSharpCornerApi.Data;
using CSharpCornerApi.Models; 

[Route("API/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly AppDbContext _context;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger, AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _context = context;
    }

   
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserModel model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} registered.", model.Email);
            await _signInManager.SignInAsync(user, isPersistent: false);
            
            // Добавляем запись в историю пользователя
            _context.UserHistories.Add(new UserHistory 
            { 
                UserId = user.Id, 
                Action = "Registration", 
                ActionDate = DateTime.UtcNow 
            });
            await _context.SaveChangesAsync();

            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in.", model.Email);
            
              // Добавляем запись в историю пользователя
            var user = await _userManager.FindByNameAsync(model.Email);
            _context.UserHistories.Add(new UserHistory 
            { 
                UserId = user.Id, 
                Action = "Login", 
                ActionDate = DateTime.UtcNow 
            });
            await _context.SaveChangesAsync();

            return Ok();
        }

        _logger.LogWarning("Failed login attempt for {Email}.", model.Email);
        return Unauthorized();
    }
}
