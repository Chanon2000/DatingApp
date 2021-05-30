using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; // "Mvc" ซึ่ง API controller ที่เราเขียนอยู่ ก็คือ "c"
using Microsoft.EntityFrameworkCore;
// "v" จะมาจากข้างนอก (angular)
//

namespace API.Controllers
{
    // #1 ใส่ attributes ให้ controller
    // [ApiController] // ApiController
    // [Route("api/[controller]")] // root // [controller] เรียกว่าเป็น placeholder แทนชื่อนำหน้าของ controllder class นี้
    // #7.2 ลบ attribute นี้ออกไปเลยเพราะเขียนที่ BaseApiController แล้ว
    public class UsersController : BaseApiController // #7.1 เปลี่ยนมารับ BaseApiController แทน ControllerBase
    // สิ่งที่ controllder ต้องทำคือ get data จาก database
    {
        // #2. สร้าง construtor
        private readonly DataContext _context;
        public UsersController(DataContext context) // inject DataContext เข้าไป
        {
            _context = context;
        }

        // #3. add endpoint
        [HttpGet]
        // #4.เปลี่ยน code เป็น async
        // public ActionResult<IEnumerable<AppUser>> GetUsers() // แบบไม่ async
        // เป็นเป็น async โดยการ
            // 1. เพิ่ม async
            // 2. หุ้ม type return ด้วย Task
        // แต่ไม่แก้เป็น async ก็จะไม่ค่อยเห็นความเปลี่ยนแปลงอะไรเนื่องจาก app เรามันเล็กอยู่แล้ว เลยไม่ดห็นว่าความรวดเร็วมันเพิ่มขึ้น
    // #15. Adding the authentication middleware
    // #15.1 เติม attribute ป้องกันเร้า
        [AllowAnonymous] // [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        // type ของสิ่งที่จะ return <IEnumerable<AppUser>>
        // IEnumerable ของ AppUser
        // หรือเราจะใช้ List แทนก็ได้ <List<AppUser>> 
            // แต่เนื่องจากว่า List มันมีบริการ search , จัดการกับข้อมูลมากมายซึ่งในที่นี้เราแค่ต้องการ return ไปให้ client ดู จึงใช้ IEnumerable ดีกว่า
        // ทั้ง IEnumerable และ List เหมือนกันคือ return list ของ user ไปให้ client
        {
        // V: แรก
            // var users = _context.Users.ToList(); //สร้าง ตัวแปรเก็บข้อมูล users
            // return users;

        // V:ลดcode
            // return _context.Users.ToList(); //สร้าง ตัวแปรเก็บข้อมูล users

        // V: asynchronous
            // นั้นคือจะทำให้ถ้ามีคน hit req นี้ มันการจะไปทำงาน thread นึง ในขณะเดียวกันมีอีกคน hit req นี้ มันก็จะสามารถส่งไปอีก thread นึงได้
            return await _context.Users.ToListAsync(); // เนื่องจาก ToList() ไม่ใช่ async method ดังนั้นจึงใช้versionที่เป็นasyncนั้นคือ ToListAsync()
            // return _context.Users.ToListAsync().Result; // ใช้ .Result แทน await
        }
    
        [Authorize] // #15 // ถ้าไม่ใส่ middleware จะ error
        // secord endpoint
        // ต้องการข้อมูล user แค่คนเดียว (GET BY ID)
        [HttpGet("{id}")]
        // public ActionResult<AppUser> GetUser(int id) // แบบไม่ async
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
        // V: แรก
            // var user = _context.Users.Find(id); // Find() หา entity โดยรับ primary key มา
            // return user;

        // V:ลดcode
            // return _context.Users.Find(id); // Find() หา entity โดยรับ primary key มา

        // V: asynchronous
            return await _context.Users.FindAsync(id);
        }
    }
}