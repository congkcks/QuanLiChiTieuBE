using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLiChiTieu.Models;

[Table("groups")]
public partial class Group
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Column("group_name")]
    [StringLength(100)]
    public string GroupName { get; set; } = null!;

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Group")]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    [InverseProperty("Group")]
    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
