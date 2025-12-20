using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLiChiTieu.Models;

[PrimaryKey("GroupId", "UserId")]
[Table("group_members")]
public partial class GroupMember
{
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    [Key]
    [Column("user_id")]
    public int UserId { get; set; }

    [Column("joined_at", TypeName = "timestamp without time zone")]
    public DateTime? JoinedAt { get; set; }

    [ForeignKey("GroupId")]
    [InverseProperty("GroupMembers")]
    public virtual Group Group { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("GroupMembers")]
    public virtual User User { get; set; } = null!;
}
