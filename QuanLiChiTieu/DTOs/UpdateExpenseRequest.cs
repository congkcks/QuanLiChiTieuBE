namespace QuanLiChiTieu.DTOs;

public class UpdateExpenseRequest
{
    public string Title { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public List<int> ParticipantIds { get; set; } = new();
    public string? Note { get; set; }
}
