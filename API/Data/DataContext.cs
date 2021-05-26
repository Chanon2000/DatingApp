using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data // นี้คือ namespace ของ DataContext (method ที่คุณเขียนด้านล่าง)
// ระวังให้ดีเวลาย้ายfilr คุณต้องเปลี่ยน namespace ด้วย
{
    // จะใช้ DbContext ต้องอ้างถึง namespace ของมันด้วย
    public class DataContext : DbContext // #1 อ้างถึง DbContext
    {
        // #2 สร้าง construtor
            // เมื่อ class ถูกเรียก construtor จะทำงาน
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        // #3 สร้าง Dbset
            // Dbset คือ type
            // ซึ่งเอา class AppUser มา สร้าง database
        // #4 add ไปที่ startup class (add configura นี้ไปที่ startup class) ซึ่งจะทำให้เรา inject DataContext ไปที่ ส่วนอื่นของ application ได้
        public DbSet<AppUser> Users { get; set; }
    }
}