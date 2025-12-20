using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLiChiTieu.Models;

[Table("expenses")]
public partial class Expense
{
    [Key]
    [Column("expense_id")]
    public int ExpenseId { get; set; }

    [Column("group_id")]
    public int GroupId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("amount")]
    [Precision(12, 2)]
    public decimal Amount { get; set; }

    [Column("expense_date")]
    public DateOnly ExpenseDate { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]

    public DateTime? CreatedAt { get; set; }
    [Column("participant_ids")]
    public string ParticipantIds { get; set; } = null!;


    [ForeignKey("CategoryId")]
    [InverseProperty("Expenses")]
    public virtual ExpenseCategory? Category { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("Expenses")]
    public virtual Group Group { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Expenses")]
    public virtual User User { get; set; } = null!;
}
