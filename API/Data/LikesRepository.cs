using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likeUserId); // เพราะ 2 key นี้รวมกันถึงจะได้ primary key (เพราะเป็น join table) 
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable(); // เหมือนใส่ OrderBy เตรียมไว้ก่อน (ไม่จำเป็นต้อง OrderBy หลังจาก select เพราะยังไงมันก็ไป execute พร้อมกันที่ return)
            var likes = _context.Likes.AsQueryable();

            if (predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.LikedUser); // จะได้ users จาก like table (LikedUser ถ้าไปดูที่ UserLike class มันจะคือ AppUser นั้นแหละ)
                // ใส่ลง users query
            }

            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser); // จะได้ list of users ที่ liked user ที่กำลัง login อยู่
            }
            
            return await users.Select(user => new LikeDto 
            // เราไม่ใช่ autoMapper เราจะใช้แค่ manual select statement เพื่อ project ตรงๆลง LikeDto // เนื่องจาก prop มันไม่เยอะ เลยคิดว่าถ้าใช้ automapper ก็หน้าจะเขียน code ประมาณนี้
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}