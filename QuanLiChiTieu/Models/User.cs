using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLiChiTieu.Models;

[Table("users")]
[Index("Email", Name = "users_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("full_name")]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    [InverseProperty("User")]
    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
