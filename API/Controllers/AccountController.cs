using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    // #8 สร้าง AccountController
    public class AccountController : BaseApiController
    {
    // #14. Creating a User DTO and returning the token
        private readonly DataContext _context;
    // #14.1 1.ใส่ITokenServiceลงพารามิเตอร์ 2.สร้างfiled  _tokenService เพื่อรับค่าจะพารามิเตอร์
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

    // #14.2 แก้ให้เป็น UserDto
    // #14.3 ไปเติม tokenKey ที่ appsetting
        // #8.1 สร้าง register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        // [FromBody] ใน พารามิเตอร์ คุณสามารถใส่ attribute เพื่อบอกได้ว่าจะเอาข้อมูลจากไหนมาใส่ parameter
        // แต่เราไม่จำเป็นต้องใส่ให้ apiController ก็ได้ เพราะว่ามันฉลาดพอที่จะรู้ว่าข้อมูลอยู่ตรงไหน
        // # 9. สร้าง DTOs และ สร้าง UserExists
        // string username, string password เปลี่ยนเป็น
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken"); //ทำแค่ return ไม่ต้อง {} ก็ได้
            // เราใช้ ActionResult เราถึง return http status ได้
            // BadRequest คือ 400 status 

            using var hmac = new HMACSHA512(); // hmac เพื่อเอาไว้สร้าง password hash
                                               // เมื่อเราเรียก HMACSHA512() class ภายใน using statement มันจะเรียก dispose method เพื่อทำการกำจัด
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key // กำหนดให้เป็น key
            };

            _context.Users.Add(user); // add method คือ insert ของ entity framework ซึ่งทำแค่นั้นจะยังไม่ถูก add เข้า database
            await _context.SaveChangesAsync(); // ทำการ track ใน entity framework เข้า database

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        // #11. Adding a login endpoint
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Find จะใช้ได้มีเมื่อเราจะหาrecord ด้วยprimary key (เนื่องจาก username ไม่ใช่ primary key)
            // SingleOrDefaultAsync มันจะปล่อย error ถ้ามี username มากกว่า 1 record
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);


            if (user == null) return Unauthorized("Invalid username"); // แสดงว่าuser ที่ login ไม่มีใน db

            // คำนวณ password hash
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }





        // private คือ ใช้ได้เฉพาะใน class นี้เท่านั้น
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}