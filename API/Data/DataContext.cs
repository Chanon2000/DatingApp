using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; } 
        // จะเห็นว่าคุณไม่ได้ add Photos มาใส่ใน DbSet เลย เพราะคุณคุณใช้ผ่าน IConllection ของ Users ตลอด
        public DbSet<UserLike> Likes { get; set; }
        // ทำการ override method ที่อยู่ใน DBcontext นั้นก็คือ OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // เนื่องจากทำการ override เราต้องอ้างอิงถึงของเก่าด้วยโดยใช้ base
            // ถ้าไม่ทำอาจทำให้เกิด error ได้ ในตอน migration

            builder.Entity<UserLike>() // ใส่ entity เป็น type parameter ที่เราต้องการจะ configure
                .HasKey(k => new {k.SourceUserId, k.LikedUserId}); // เอา SourceUserId กับ LikedUserId รวมกันเป็น Primary key ของ table นี้
                // ถ้าเราไม่กำหนด primary key ให้ entity นี้ เราจะต้องกำหนด key ด้วยตัวเอง

            // configure the relationships inside
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers) // 1 SourceUser สามารถ like LikedUsers หลายคนได้
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade); // นั้นทำให้เมื่อคุณ delete user มันจะ delete related entities // ถ้าเป็น sql server คุณต้องใส่เป็น DeleteBehavior.NoAction ถ้าไม่เป็นแบบนี้อาจทำให้เกิด error ตอน migration ได้

            // อีกด้านนึงของ UserLike ด้านบน
            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers) // user ที่ถูก like (LikedUser) สามารถถูก like โดย LikedByUsers ได้หลายคน
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    } 
}

