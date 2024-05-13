using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("API/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
   public class UserModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}   
[HttpPost("register")]
public async Task<IActionResult> Register(UserModel model)
{
    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
    var result = await _userManager.CreateAsync(user, model.Password);

    if (result.Succeeded)
    {
        await _signInManager.SignInAsync(user, isPersistent: false);
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
        return Ok();
    }

    return Unauthorized();
}

}
