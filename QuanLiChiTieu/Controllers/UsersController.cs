using Microsoft.AspNetCore.Mvc;
using QuanLiChiTieu.Models;

namespace QuanLiChiTieu.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Users.ToList());
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return Ok(user);
    }

}
