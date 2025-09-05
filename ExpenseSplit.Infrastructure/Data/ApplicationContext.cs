using ExpenseSplit.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplit.Infrastructure.Data;

public class ApplicationContext : IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable(nameof(User));
        builder.Entity<IdentityRole>().ToTable("Role");
        builder.Entity<Expense>().ToTable("Expenses");
        builder.Entity<SplitDetail>().ToTable("SplitDetails");

        builder.Entity<UserGroupRef>()
            .HasOne(x => x.GroupDetail)
            .WithMany(x => x.UserGroupRefs)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GroupDetail>()
            .HasOne(x => x.User)
            .WithMany(x => x.GroupDetails)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GroupDetail>()
            .HasOne(x => x.GroupCategoryMaster)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Expense>()
            .HasOne(x => x.GroupDetail)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Expense>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.PaidById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SplitDetail>()
            .HasOne(x => x.Expense)
            .WithMany(x => x.SplitDetails)
            .HasForeignKey(x => x.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SplitDetail>()
               .HasOne(x => x.User)
               .WithMany(x => x.SplitDetails)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        //Invitation
        builder.Entity<Invitation>()
               .HasOne(i => i.Group)
               .WithMany(g => g.Invitations)
               .HasForeignKey(i => i.GroupId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Invitation>()
            .HasOne(i => i.InviterUser)
            .WithMany()                          // you can add collections if you want
            .HasForeignKey(i => i.InviterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Invitation>()
            .HasOne(i => i.InvitedUser)
            .WithMany()
            .HasForeignKey(i => i.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique pending invitation per (Group, InvitedUser)
        builder.Entity<Invitation>()
         .HasIndex(i => new { i.GroupId, i.InvitedUserId, i.Status })
         .HasFilter("[Status] = 0")             // 0 = Pending
         .IsUnique();

        // (Optional) speed up lookups
        builder.Entity<UserGroupRef>()
         .HasIndex(x => new { x.GroupId, x.UserId })
         .IsUnique(); // a user can be a member once per group
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserGroupRef> UserGroups => Set<UserGroupRef>();
    public DbSet<GroupCategoryMaster> GroupCategoryMasters => Set<GroupCategoryMaster>();
    //public DbSet<GroupCategoryRef> GroupCategoryRefs { get; set; }
    public DbSet<GroupDetail> GroupDetails  => Set<GroupDetail>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<SplitDetail> SplitDetails => Set<SplitDetail>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
}
