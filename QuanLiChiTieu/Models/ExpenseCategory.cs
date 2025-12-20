using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLiChiTieu.Models;

[Table("expense_categories")]
public partial class ExpenseCategory
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("category_name")]
    [StringLength(50)]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
