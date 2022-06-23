using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data // นี้คือ namespace ของ DataContext (method ที่คุณเขียนด้านล่าง)
{
    public class DataContext : DbContext // #1 อ้างถึง DbContext
    {
        // #2 สร้าง construtor
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }   // #3 สร้าง Dbset
                                                    // #4 add ไปที่ startup class 
    }
}

