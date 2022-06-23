using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    // #1 ใส่ attributes ให้ controller
    // #7.2 ลบ attribute นี้ออกไปเลยเพราะเขียนที่ BaseApiController แล้ว
    public class UsersController : BaseApiController // #7.1 เปลี่ยนมารับ BaseApiController แทน ControllerBase
    {
        // #2. สร้าง construtor
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        // #3. add endpoint
        [HttpGet]
    // #15. Adding the authentication middleware
    // #15.1 เติม attribute ป้องกันเร้า
        [AllowAnonymous] // [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() // #4.เปลี่ยน code เป็น async
        {
            return await _context.Users.ToListAsync();
        }
    
        [Authorize] // #15 // ถ้าไม่ใส่ middleware จะ error
        // secord endpoint
        [HttpGet("{id}")] // ต้องการข้อมูล user แค่คนเดียว (GET BY ID)
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}