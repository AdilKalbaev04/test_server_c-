using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("API/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UsersController> _logger; 

    public UsersController(UserManager<IdentityUser> userManager, ILogger<UsersController> logger) 
    {
        _userManager = userManager;
        _logger = logger; 
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IdentityUser>>> GetUsers()
    {
        return Ok(await _userManager.Users.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IdentityUser>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, IdentityUser user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

          _logger.LogInformation("User updated: {Id}", id);

        return NoContent();
    }

  [HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(string id)
{
    var user = await _userManager.FindByIdAsync(id);

    if (user == null)
    {
        return NotFound();
    }

    var result = await _userManager.DeleteAsync(user);

    if (!result.Succeeded)
    {
        return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
    
    _logger.LogInformation("User deleted: {Id}", id); 
    return NoContent();
}
}
