
using Microsoft.EntityFrameworkCore;

namespace QuanLiChiTieu.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=aws-1-ap-southeast-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.kjkpftwfbsxlhqhozmcq;Password=123456;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS", "VECTOR" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId)
                .HasName("expenses_pkey");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ✅ CỘT BẠN THÊM
            entity.Property(e => e.ParticipantIds)
                .HasColumnName("participant_ids")
                .HasColumnType("text")
                .IsRequired();

            // ✅ FK: category_id (nullable)
            entity.HasOne(d => d.Category)
                .WithMany(p => p.Expenses)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("expenses_category_id_fkey");

            // ✅ FK: group_id (NOT NULL)
            entity.HasOne(d => d.Group)
                .WithMany(p => p.Expenses)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("expenses_group_id_fkey");

            // ✅ FK: user_id (buyer)
            entity.HasOne(d => d.User)
                .WithMany(p => p.Expenses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("expenses_user_id_fkey");
        });


        modelBuilder.Entity<ExpenseCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("expense_categories_pkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("groups_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.UserId }).HasName("group_members_pkey");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers).HasConstraintName("group_members_group_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers).HasConstraintName("group_members_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<User1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", "auth", tb => tb.HasComment("Auth: Stores user login data within a secure schema."));

            entity.HasIndex(e => e.ConfirmationToken, "confirmation_token_idx")
                .IsUnique()
                .HasFilter("((confirmation_token)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.EmailChangeTokenCurrent, "email_change_token_current_idx")
                .IsUnique()
                .HasFilter("((email_change_token_current)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.EmailChangeTokenNew, "email_change_token_new_idx")
                .IsUnique()
                .HasFilter("((email_change_token_new)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.ReauthenticationToken, "reauthentication_token_idx")
                .IsUnique()
                .HasFilter("((reauthentication_token)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.RecoveryToken, "recovery_token_idx")
                .IsUnique()
                .HasFilter("((recovery_token)::text !~ '^[0-9 ]*$'::text)");

            entity.HasIndex(e => e.Email, "users_email_partial_key")
                .IsUnique()
                .HasFilter("(is_sso_user = false)");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ConfirmedAt).HasComputedColumnSql("LEAST(email_confirmed_at, phone_confirmed_at)", true);
            entity.Property(e => e.EmailChangeConfirmStatus).HasDefaultValue((short)0);
            entity.Property(e => e.EmailChangeTokenCurrent).HasDefaultValueSql("''::character varying");
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.IsSsoUser)
                .HasDefaultValue(false)
                .HasComment("Auth: Set this column to true when the account comes from SSO. These accounts can have duplicate emails.");
            entity.Property(e => e.Phone).HasDefaultValueSql("NULL::character varying");
            entity.Property(e => e.PhoneChange).HasDefaultValueSql("''::character varying");
            entity.Property(e => e.PhoneChangeToken).HasDefaultValueSql("''::character varying");
            entity.Property(e => e.ReauthenticationToken).HasDefaultValueSql("''::character varying");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
