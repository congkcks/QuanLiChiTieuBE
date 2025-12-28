using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiChiTieu.Models;

namespace QuanLiChiTieu.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Dashboard chia tiền theo tháng (tính động)
        /// </summary>
        [HttpGet("summary")]
        public IActionResult GetMonthlySummary(int groupId, int year, int month)
        {
            // 1. Lấy hóa đơn trong tháng
            var expenses = _context.Expenses
                .Where(e => e.GroupId == groupId &&
                            e.ExpenseDate.Year == year &&
                            e.ExpenseDate.Month == month)
                .ToList();

            // 2. Lấy thành viên trong nhóm
            var members = _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => gm.User)
                .ToList();

            var result = members.Select(member =>
            {
                decimal totalPaid = expenses
                    .Where(e => e.UserId == member.UserId)
                    .Sum(e => e.Amount);

                decimal totalShare = 0;

                foreach (var expense in expenses)
                {
                    var participantIds = expense.ParticipantIds
                        .Split(',')
                        .Select(int.Parse)
                        .ToList();

                    if (participantIds.Contains(member.UserId))
                    {
                        totalShare += expense.Amount / participantIds.Count;
                    }
                }

                return new
                {
                    userId = member.UserId,
                    fullName = member.FullName,
                    paid = totalPaid,
                    mustPay = totalShare,
                    balance = totalPaid - totalShare
                };
            });

            return Ok(result);
        }
        [HttpGet("by-month")]
        public IActionResult GetExpensesByMonth(int groupId, int year, int month)
        {
            var expenses = _context.Expenses
              .Where(e => e.GroupId == groupId &&
                e.ExpenseDate.Year == year &&
                e.ExpenseDate.Month == month)
               .Include(e => e.User)
               .ToList();
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
                    createdBy = e.User.FullName,
                    participants = participantIds,
                    costPerPerson = e.Amount / participantIds.Count
                };
            }).ToList();

            return Ok(result);






        }



    }
}
