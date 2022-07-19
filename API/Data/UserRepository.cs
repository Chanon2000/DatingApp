using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
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
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            // เราจะไม่ทำในแต่ละ method แบบนี้ เพราะมันไม่มีประสิทธิภาพ
            var query = _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsNoTracking(); // เพื่อให้ efficient มากยิ่งขึ้น โดย default แล้วเมื่อเราเข้าไป get entities จาก entity framework โดย entity framework จะทำการ tracking entities เหล่านั้น 
                // โดยในที่นี้เราแค่ต้องการอ่านเฉยๆ เราไม่ได้จะทำอะไรกับ entity ดังนั้นการใส่ .AsNoTracking() เพื่อทำการปิด tracking ใน entity framework (อ่านเฉยๆ แต่ไม่ดึงข้อมูลออกมา)

                // คำอธิบายจาก comment ใน udemy
                // Any entities received from EF will be 'tracked' by default. This means if you make a change to an entity you only need to call SaveChanges and the query will be made to the DB to update it. "AsNoTracking" means that EF will give you the entity but then immediately forget about it.

            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize); // สังเกตว่า ทำการ execute query ที่ CreateAsync
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
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}