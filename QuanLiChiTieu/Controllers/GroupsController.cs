using Microsoft.AspNetCore.Mvc;
using QuanLiChiTieu.DTOs;
using QuanLiChiTieu.Models;

namespace QuanLiChiTieu.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult GetAllGroups()
    {
        var groups = _context.Groups
            .Select(g => new
            {
                groupId = g.GroupId,
                groupName = g.GroupName,
                memberCount = _context.GroupMembers.Count(m => m.GroupId == g.GroupId)
            })
            .ToList();

        return Ok(groups);
    }
    [HttpGet("enter")]
    public IActionResult EnterGroup(int groupId, int userId)
    {
        // 1. Kiểm tra nhóm tồn tại
        var group = _context.Groups
            .FirstOrDefault(g => g.GroupId == groupId);

        if (group == null)
            return NotFound("Group not found");

        // 2. Kiểm tra user có thuộc nhóm không
        var isMember = _context.GroupMembers
            .Any(gm => gm.GroupId == groupId && gm.UserId == userId);

        if (!isMember)
            return Unauthorized("User is not a member of this group");

        // 3. Lấy danh sách thành viên trong nhóm
        var members = _context.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => new
            {
                gm.User.UserId,
                gm.User.FullName,
                gm.User.Email
            })
            .ToList();

        return Ok(new
        {
            groupId = group.GroupId,
            groupName = group.GroupName,
            members = members
        });
    }
    [HttpPost("join")]
    public IActionResult JoinGroup([FromBody] JoinGroupRequest request)
    {
        // 1. Kiểm tra user tồn tại
        var userExists = _context.Users.Any(u => u.UserId == request.UserId);
        if (!userExists)
            return NotFound("User not found");

        // 2. Kiểm tra group tồn tại
        var groupExists = _context.Groups.Any(g => g.GroupId == request.GroupId);
        if (!groupExists)
            return NotFound("Group not found");

        // 3. Kiểm tra đã là thành viên chưa
        var alreadyJoined = _context.GroupMembers.Any(gm =>
            gm.GroupId == request.GroupId &&
            gm.UserId == request.UserId);

        if (alreadyJoined)
            return BadRequest("User already joined this group");

        // 4. Thêm vào bảng group_members
        var member = new GroupMember
        {
            GroupId = request.GroupId,
            UserId = request.UserId
        };

        _context.GroupMembers.Add(member);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Join group successfully",
            groupId = request.GroupId,
            userId = request.UserId
        });
    }
    [HttpPost("create")]
    public IActionResult CreateGroup([FromBody] CreateGroupRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.GroupName))
            return BadRequest("Group name is required");

        // 1. Kiểm tra user tồn tại
        var userExists = _context.Users.Any(u => u.UserId == request.UserId);
        if (!userExists)
            return NotFound("User not found");

        // ⭐ 2. KIỂM TRA USER ĐÃ Ở TRONG NHÓM CHƯA
        var alreadyInGroup = _context.GroupMembers
            .Any(gm => gm.UserId == request.UserId);

        if (alreadyInGroup)
            return BadRequest("User already belongs to a group and cannot create a new one");

        // 3. Tạo nhóm mới
        var group = new Group
        {
            GroupName = request.GroupName
        };

        _context.Groups.Add(group);
        _context.SaveChanges();

        // 4. Thêm user vào group_members
        var member = new GroupMember
        {
            GroupId = group.GroupId,
            UserId = request.UserId
        };

        _context.GroupMembers.Add(member);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Group created successfully",
            groupId = group.GroupId,
            groupName = group.GroupName
        });
    }
    [HttpGet("by-user/{userId}")]
    public IActionResult GetGroupsByUser(int userId)
    {
        // Kiểm tra user tồn tại
        var userExists = _context.Users.Any(u => u.UserId == userId);
        if (!userExists)
            return NotFound("User not found");

        var groups = _context.GroupMembers
            .Where(gm => gm.UserId == userId)
            .Select(gm => new
            {
                gm.Group.GroupId,
                gm.Group.GroupName
            })
            .ToList();

        return Ok(groups);
    }
    [HttpGet("{groupId}/members")]
    public IActionResult GetGroupMembers(int groupId)
    {
        // Kiểm tra group tồn tại
        var groupExists = _context.Groups.Any(g => g.GroupId == groupId);
        if (!groupExists)
            return NotFound("Group not found");

        var members = _context.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => new
            {
                gm.User.UserId,
                gm.User.FullName,
                gm.User.Email
            })
            .ToList();

        return Ok(members);
    }
    [HttpPost("leave")]
    public IActionResult LeaveGroup([FromBody] LeaveGroupRequest request)
    {
        // 1. Kiểm tra user có trong nhóm không
        var member = _context.GroupMembers
            .FirstOrDefault(gm =>
                gm.GroupId == request.GroupId &&
                gm.UserId == request.UserId);

        if (member == null)
            return NotFound("User is not a member of this group");

        // 2. Xóa khỏi group_members
        _context.GroupMembers.Remove(member);
        _context.SaveChanges();

        return Ok(new
        {
            message = "User left the group successfully"
        });
    }



}
