using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager) // ใช้ userManager เพื่อสร้าง user แทน
        {
            if (await userManager.Users.AnyAsync()) return; // userManager มันก็มี Users เหมือนกัน // Users ให้เราสามารถเข้าถึง user table ได้

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if (users == null) return; // คุณลืมเขียน if นี้ใน session เก่าๆ

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                // "Pa$$w0rd" เราทำการ hardcode password // และเนื่องจากคุณปิด RequireNonAlphanumeric ของ password ไปแล้ว ทำให้เราสามารถใส่ password แบบนี้ไม่ซับซ้อนแบบนี้ได้
                await userManager.AddToRoleAsync(user, "Member");
            }

            // สร้าง admin
            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});

            // await context.SaveChangesAsync(); // เนื่องจาก userManager จัดการเรื่อง SaveChangesAsync ให้แล้ว
        }
    }
}