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
        // เนื่องจาก เราจะสื่อสาร photos เป็น users collection เท่านั้น เลยไม่จำเป็นต้องมี Dbset
        
    }
}

