using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly AssetContext _context;

    public UserController(AssetContext context)
    {
        _context = context;
    }

    // GET: api/User/users
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsers()
    {
        try
        {
            var users = await _context.DtUsers
             .Include(u => u.RoleNavigation) // Join with DtUserRole
             .Select(u => new
             {
                 u.Id,
                 u.Email,
                 Role = u.RoleNavigation.Role, // Get role name
                 u.CreatedDate,
                 u.ModifiedDate,
                 u.CreatedBy
             })
             .ToListAsync();

            return Ok(users);
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // GET: api/User/users/5
    [HttpGet("users/{id}")]
    public async Task<ActionResult<object>> GetUser(int id)
    {
        var user = await _context.DtUsers
            .Include(u => u.RoleNavigation)
            .Where(u => u.Id == id)
            .Select(u => new
            {
                u.Id,
                u.Email,
                Role = u.RoleNavigation.Id, // Return role ID for editing
                RoleName = u.RoleNavigation.Role, // Role name for display
                u.CreatedDate,
                u.ModifiedDate,
                u.CreatedBy
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // GET: api/User/roles
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<object>>> GetUserRoles()
    {
        var roles = await _context.DtUserRoles
            .Select(r => new
            {
                r.Id,
                r.Role,
                r.Description
            })
            .ToListAsync();

        return Ok(roles);
    }

    // POST: api/User/users
    [HttpPost("users")]
    public async Task<ActionResult<DtUser>> CreateUser(UserCreateDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if email already exists
        if (await _context.DtUsers.AnyAsync(u => u.Email == userDto.Email))
        {
            return BadRequest("Email already exists");
        }

        var user = new DtUser
        {
            Email = userDto.Email,
            Role = userDto.Role,
            CreatedDate = DateTime.Now,
            CreatedBy = "System" // You can get this from the current user's token
        };

        _context.DtUsers.Add(user);
        await _context.SaveChangesAsync();

        // Return the created user with role name
        var createdUser = await _context.DtUsers
            .Include(u => u.RoleNavigation)
            .Where(u => u.Id == user.Id)
            .Select(u => new
            {
                u.Id,
                u.Email,
                Role = u.RoleNavigation.Role,
                u.CreatedDate,
                u.CreatedBy
            })
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, createdUser);
    }

    // PUT: api/User/users/5
    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userDto)
    {
        if (id != userDto.Id)
        {
            return BadRequest();
        }

        var user = await _context.DtUsers.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Check if email already exists (excluding current user)
        if (await _context.DtUsers.AnyAsync(u => u.Email == userDto.Email && u.Id != id))
        {
            return BadRequest("Email already exists");
        }

        user.Email = userDto.Email;
        user.Role = userDto.Role;
        user.ModifiedDate = DateTime.Now;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/User/users/5
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.DtUsers.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.DtUsers.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return _context.DtUsers.Any(e => e.Id == id);
    }
}

// DTOs for user operations
public class UserCreateDto
{
    public string Email { get; set; }
    public int? Role { get; set; }
}

public class UserUpdateDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public int? Role { get; set; }
}