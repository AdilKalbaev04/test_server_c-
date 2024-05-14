using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CSharpCornerApi.Data;
using CSharpCornerApi.Models; 
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;


[Route("API/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<AccountController> _logger;
    private readonly AppDbContext _context;

    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly string _jwtKey;


public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger, AppDbContext context, RoleManager<IdentityRole> roleManager,IConfiguration configuration)
{
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _context = context;
            _roleManager = roleManager; 
                _jwtKey = configuration["Jwt:Key"]; 


    }

   
[HttpPost("register")]
public async Task<IActionResult> Register(UserModel model)
{

     if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
    var result = await _userManager.CreateAsync(user, model.Password);

    if (result.Succeeded)
    {
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        await _userManager.AddToRoleAsync(user, "User");

        _logger.LogInformation("User {Email} registered.", model.Email);
        await _signInManager.SignInAsync(user, isPersistent: false);
        
        _context.UserHistories.Add(new UserHistory 
        { 
            UserId = user.Id, 
            Action = "Registration", 
            ActionDate = DateTime.UtcNow 
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
}


  [HttpPost("login")]
public async Task<IActionResult> Login(UserModel model)
{
    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

    if (result.Succeeded)
    {
        _logger.LogInformation("User {Email} logged in.", model.Email);
        
        var user = await _userManager.FindByNameAsync(model.Email);
        _context.UserHistories.Add(new UserHistory 
        { 
            UserId = user.Id, 
            Action = "Login", 
            ActionDate = DateTime.UtcNow 
        });
        await _context.SaveChangesAsync();

        var tokenHandler = new JwtSecurityTokenHandler();
     var key = Encoding.ASCII.GetBytes(_jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return Ok(new { jwt });
    }

    _logger.LogWarning("Failed login attempt for {Email}.", model.Email);
    return Unauthorized();
}

[HttpPost("addAdminRole/{userId}")]
public async Task<IActionResult> AddAdminRole(string userId)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return NotFound("User not found.");
    }

    if (!await _roleManager.RoleExistsAsync("Admin"))
    {
        await _roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var addToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
    if (addToRoleResult.Succeeded)
    {
        return Ok("Role 'Admin' added to user.");
    }

    return BadRequest(string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
}

[HttpPost("addRole/{userId}/{role}")]
public async Task<IActionResult> AddRoleToUser(string userId, string role)
{
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        return NotFound("User not found.");
    }

    if (!await _roleManager.RoleExistsAsync(role))
    {
        await _roleManager.CreateAsync(new IdentityRole(role));
    }

    var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
    if (addToRoleResult.Succeeded)
    {
        return Ok($"Role '{role}' added to user.");
    }

    return BadRequest(string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
}

}