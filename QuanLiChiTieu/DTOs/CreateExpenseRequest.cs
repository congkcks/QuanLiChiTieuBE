namespace QuanLiChiTieu.DTOs;

public class CreateExpenseRequest
{
    public int GroupId { get; set; }
    public int BuyerId { get; set; }  
    public string Title { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }

    // danh sách user ăn, ví dụ: [1,2,3]
    public List<int> ParticipantIds { get; set; } = new();
    public string? Note { get; set; }
}
