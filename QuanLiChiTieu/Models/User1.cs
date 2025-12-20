using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLiChiTieu.Models;

/// <summary>
/// Auth: Stores user login data within a secure schema.
/// </summary>
[Table("users", Schema = "auth")]
[Index("InstanceId", Name = "users_instance_id_idx")]
[Index("IsAnonymous", Name = "users_is_anonymous_idx")]
[Index("Phone", Name = "users_phone_key", IsUnique = true)]
public partial class User1
{
    [Column("instance_id")]
    public Guid? InstanceId { get; set; }

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("aud")]
    [StringLength(255)]
    public string? Aud { get; set; }

    [Column("role")]
    [StringLength(255)]
    public string? Role { get; set; }

    [Column("email")]
    [StringLength(255)]
    public string? Email { get; set; }

    [Column("encrypted_password")]
    [StringLength(255)]
    public string? EncryptedPassword { get; set; }

    [Column("email_confirmed_at")]
    public DateTime? EmailConfirmedAt { get; set; }

    [Column("invited_at")]
    public DateTime? InvitedAt { get; set; }

    [Column("confirmation_token")]
    [StringLength(255)]
    public string? ConfirmationToken { get; set; }

    [Column("confirmation_sent_at")]
    public DateTime? ConfirmationSentAt { get; set; }

    [Column("recovery_token")]
    [StringLength(255)]
    public string? RecoveryToken { get; set; }

    [Column("recovery_sent_at")]
    public DateTime? RecoverySentAt { get; set; }

    [Column("email_change_token_new")]
    [StringLength(255)]
    public string? EmailChangeTokenNew { get; set; }

    [Column("email_change")]
    [StringLength(255)]
    public string? EmailChange { get; set; }

    [Column("email_change_sent_at")]
    public DateTime? EmailChangeSentAt { get; set; }

    [Column("last_sign_in_at")]
    public DateTime? LastSignInAt { get; set; }

    [Column("raw_app_meta_data", TypeName = "jsonb")]
    public string? RawAppMetaData { get; set; }

    [Column("raw_user_meta_data", TypeName = "jsonb")]
    public string? RawUserMetaData { get; set; }

    [Column("is_super_admin")]
    public bool? IsSuperAdmin { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("phone")]
    public string? Phone { get; set; }

    [Column("phone_confirmed_at")]
    public DateTime? PhoneConfirmedAt { get; set; }

    [Column("phone_change")]
    public string? PhoneChange { get; set; }

    [Column("phone_change_token")]
    [StringLength(255)]
    public string? PhoneChangeToken { get; set; }

    [Column("phone_change_sent_at")]
    public DateTime? PhoneChangeSentAt { get; set; }

    [Column("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [Column("email_change_token_current")]
    [StringLength(255)]
    public string? EmailChangeTokenCurrent { get; set; }

    [Column("email_change_confirm_status")]
    public short? EmailChangeConfirmStatus { get; set; }

    [Column("banned_until")]
    public DateTime? BannedUntil { get; set; }

    [Column("reauthentication_token")]
    [StringLength(255)]
    public string? ReauthenticationToken { get; set; }

    [Column("reauthentication_sent_at")]
    public DateTime? ReauthenticationSentAt { get; set; }

    /// <summary>
    /// Auth: Set this column to true when the account comes from SSO. These accounts can have duplicate emails.
    /// </summary>
    [Column("is_sso_user")]
    public bool IsSsoUser { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("is_anonymous")]
    public bool IsAnonymous { get; set; }
}
