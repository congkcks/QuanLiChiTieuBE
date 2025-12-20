using System.ComponentModel.DataAnnotations;

namespace QuanLiChiTieu.DTOs;
public class UpdateGroupRequest
{
    [Required]
    [StringLength(100)]
    public string GroupName { get; set; } = null!;
}
