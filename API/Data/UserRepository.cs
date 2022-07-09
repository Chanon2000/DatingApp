using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos) // ใส่ Include แล้วใส่ expression ที่ดึงข้อมูล Photos มาด้วย
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; // ต้องมี change ถึงจะ return true
            // สังเกต SaveChangesAsync() มัน return int นั้นทำให้เราเทียบแบบนี้ได้
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; // คือการให้ entity framework update และ เพิ่ม flag ไปที่ entity (เพื่อบอกว่าสิ่งต่างๆนั้นถูก modified แล้ว)
        }
    }
}