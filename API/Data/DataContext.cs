using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // install  Microsoft.AspNetCore.Identity.EntityFrameworkCore ก่อน คุณถึงจะ import ได้
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    // ใส่ parameter ให้ IdentityDbContext
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    // TUser is AppUser
    // TRole is AppRole => ไม่ต้องใส่ <int> เป็น AppRole<int> เพราะเรากำหนดที่ class มันแล้ว
    // TKey is int
    // ...
    {

        public DataContext(DbContextOptions options) : base(options)
        {
        }
        // เนื่อจจาก IdentityDbContext เตรียม table ที่เราต้องใช้ให้ด้วย
        // public DbSet<AppUser> Users { get; set; } // คุณจะเห็น warning เพราะ IdentityDbContext เตรียม Users table ให้เราแล้ว การที่คุณกำหนดมันตรงนี้จะเป็นการ override
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // config AppUser กับ AppRole

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User) // 1 User มีได้หลาย UserRoles
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role) // 1 Role มีได้หลาย UserRoles
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<UserLike>()
                .HasKey(k => new {k.SourceUserId, k.LikedUserId});

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    } 
}

