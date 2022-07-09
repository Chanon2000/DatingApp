using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed // เพื่อเอา data จาก json ใส่ลง database
    {
        // ให้เป็น static เพื่อเราจะได้ไม่ต้องสร้าง instance ใหม่ที่ class อื่นเมื่อจะใช้งาน method นี้
        public static async Task SeedUsers(DataContext context)
        // ถ้า return void หรือไม่ return อะไรเลย เขียนแบบนี้ได้ public static async Task ...
        {
            // check ว่า users table มี user มั้ย
            if (await context.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            // JsonSerializer มีตั้งแต่ .net 3 แล้ว ดังนั้นเราใช้ using System.Text.Json ได้เลย ไม่ต้องใช้ using Newtonsoft.Json (เพราะใช้ของ .net มันก็จะ native กว่าและทำงานได้ดีกว่า)
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")); // hard code password ไปเลย

                context.Users.Add(user); // ไม่ต้อง await ตรงนี้เพราะว่า เราแค่ tracking (ติดตาม) โดยใช้ entity framework (เรายังไม่ได้ทำอะไรกับ database)
            }

            await context.SaveChangesAsync();
        }
    }
}