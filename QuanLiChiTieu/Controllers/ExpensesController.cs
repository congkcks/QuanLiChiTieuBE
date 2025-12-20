using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiChiTieu.DTOs;
using QuanLiChiTieu.Models;

namespace QuanLiChiTieu.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExpensesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tạo hóa đơn chi tiêu trong một nhóm
    /// </summary>
    [HttpPost("create")]
    public IActionResult CreateExpense([FromBody] CreateExpenseRequest request)
    {
        // 1. Validate cơ bản
        if (request.ParticipantIds == null || request.ParticipantIds.Count == 0)
            return BadRequest("Participant list cannot be empty");

        // 2. Kiểm tra group tồn tại
        var groupExists = _context.Groups.Any(g => g.GroupId == request.GroupId);
        if (!groupExists)
            return NotFound("Group not found");

        // 3. Kiểm tra buyer có thuộc group không
        var isMember = _context.GroupMembers.Any(gm =>
            gm.GroupId == request.GroupId &&
            gm.UserId == request.BuyerId);

        if (!isMember)
            return Unauthorized("Buyer is not a member of this group");

        // 4. Lưu expense
        var expense = new Expense
        {
            GroupId = request.GroupId,
            UserId = request.BuyerId,
            Title = request.Title,
            Amount = request.Amount,
            ExpenseDate = request.ExpenseDate,
            ParticipantIds = string.Join(",", request.ParticipantIds),
            Note = request.Note
        };

        _context.Expenses.Add(expense);
        _context.SaveChanges();

        // 5. Trả kết quả
        return Ok(new
        {
            message = "Expense created successfully",
            expenseId = expense.ExpenseId,
            title = expense.Title,
            amount = expense.Amount,
            participants = request.ParticipantIds,
            costPerPerson = expense.Amount / request.ParticipantIds.Count
        });
    }
    [HttpGet("filter")]
    public IActionResult GetExpenses(
     [FromQuery] int groupId,
     [FromQuery] string? fromDate,
     [FromQuery] string? toDate)
    {
        // 1. Query gốc theo group
        var query = _context.Expenses
            .Where(e => e.GroupId == groupId)
            .Include(e => e.User)
            .AsQueryable();

        // 2. Nếu có fromDate → lọc
        if (!string.IsNullOrWhiteSpace(fromDate))
        {
            if (!DateOnly.TryParse(fromDate, out var from))
                return BadRequest("Invalid fromDate format. Use yyyy-MM-dd");

            query = query.Where(e => e.ExpenseDate >= from);
        }

        // 3. Nếu có toDate → lọc
        if (!string.IsNullOrWhiteSpace(toDate))
        {
            if (!DateOnly.TryParse(toDate, out var to))
                return BadRequest("Invalid toDate format. Use yyyy-MM-dd");

            query = query.Where(e => e.ExpenseDate <= to);
        }

        // 4. Load data
        var expenses = query
            .OrderByDescending(e => e.ExpenseDate)
            .ToList(); // ⭐ xử lý Split sau

        // 5. Map kết quả (an toàn với Split)
        var users = _context.Users.ToDictionary(u => u.UserId);

        var result = expenses.Select(e =>
        {
            var participantIds = e.ParticipantIds
                .Split(',')
                .Select(int.Parse)
                .ToList();

            return new
            {
                expenseId = e.ExpenseId,
                title = e.Title,
                amount = e.Amount,
                expenseDate = e.ExpenseDate,

                createdBy = new
                {
                    userId = e.User.UserId,
                    fullName = e.User.FullName,
                    email = e.User.Email
                },

                participants = participantIds
                    .Where(id => users.ContainsKey(id))
                    .Select(id => new
                    {
                        userId = id,
                        fullName = users[id].FullName
                    })
                    .ToList(),

                costPerPerson = e.Amount / participantIds.Count,
                note = e.Note
            };
        });

        return Ok(result);
    }
    [HttpGet("{expenseId}")]
    public IActionResult GetExpenseDetail(int expenseId)
    {
        var expense = _context.Expenses
            .Include(e => e.User)
            .FirstOrDefault(e => e.ExpenseId == expenseId);

        if (expense == null)
            return NotFound("Expense not found");

        var users = _context.Users.ToDictionary(u => u.UserId);

        var participantIds = expense.ParticipantIds
            .Split(',')
            .Select(int.Parse)
            .ToList();

        return Ok(new
        {
            expenseId = expense.ExpenseId,
            title = expense.Title,
            amount = expense.Amount,
            expenseDate = expense.ExpenseDate,
            note = expense.Note,

            createdBy = new
            {
                userId = expense.User.UserId,
                fullName = expense.User.FullName,
                email = expense.User.Email
            },

            participants = participantIds
                .Where(id => users.ContainsKey(id))
                .Select(id => new
                {
                    userId = id,
                    fullName = users[id].FullName
                })
                .ToList(),

            costPerPerson = expense.Amount / participantIds.Count
        });
    }
    [HttpPut("{expenseId}")]
    public IActionResult UpdateExpense(
    int expenseId,
    [FromBody] UpdateExpenseRequest request)
    {
        var expense = _context.Expenses
            .FirstOrDefault(e => e.ExpenseId == expenseId);

        if (expense == null)
            return NotFound("Expense not found");

        if (request.ParticipantIds == null || request.ParticipantIds.Count == 0)
            return BadRequest("Participant list cannot be empty");

        expense.Title = request.Title;
        expense.Amount = request.Amount;
        expense.ExpenseDate = request.ExpenseDate;
        expense.ParticipantIds = string.Join(",", request.ParticipantIds);
        expense.Note = request.Note;

        _context.SaveChanges();

        return Ok(new
        {
            message = "Expense updated successfully"
        });
    }
    [HttpDelete("{expenseId}")]
    public IActionResult DeleteExpense(int expenseId)
    {
        var expense = _context.Expenses
            .FirstOrDefault(e => e.ExpenseId == expenseId);

        if (expense == null)
            return NotFound("Expense not found");

        _context.Expenses.Remove(expense);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Expense deleted successfully"
        });
    }




}






