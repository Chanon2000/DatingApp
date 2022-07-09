using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            // *** ทำเพื่อ Select เฉพาะ property ที่เราต้องการเท่านั้น จาก database

            // เราจะทำการ optimizations โดยการให้มัน query เฉพาะ column ที่จะใช้ นั้นคือตัด PasswordHash, PasswordSalt ไม่ให้ทำการ query
            // แบบใช้ Select ทำแบบ manual
            // .Select(user => new MemberDto // แล้วทำการ map ด้วยตัวเองเลย
                // {
                //     Id = user.Id,
                //     Username = user.UserName,
                     
                // }).SingleOrDefaultAsync(); // SingleOrDefaultAsync มันทำการ execute query ด้วย

            // ใช้ ProjectTo
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // _mapper.ConfigurationProvider เข้าไปเอา Configuration ใน AutoMapperProfiles ให้เรา
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
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