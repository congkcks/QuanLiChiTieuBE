using Microsoft.AspNetCore.Mvc;
using QuanLiChiTieu.Models;
namespace QuanLiChiTieu.Controllers;
using QuanLiChiTieu.DTOs;
[ApiController]
[Route("auth")]
public class AuthController : Controller
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context)
    {
        _context = context;
    }
    [HttpPost("login")]
     public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required");

        var user = _context.Users
            .FirstOrDefault(u => u.Email == request.Email);

        if (user == null)
            return NotFound("User not found");

        return Ok(new
        {
            userId = user.UserId,
            fullName = user.FullName,
            email = user.Email
        });
    }
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("FullName and Email are required");
        }

        // 1. Kiểm tra email trùng
        var emailExists = _context.Users.Any(u => u.Email == request.Email);
        if (emailExists)
            return BadRequest("Email already exists");

        // 2. Tạo user mới
        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Register successfully",
            userId = user.UserId,
            fullName = user.FullName,
            email = user.Email
        });
    }

}
