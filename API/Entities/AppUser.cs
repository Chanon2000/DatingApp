using System;
using System.Collections.Generic;
using API.Extensions;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; } 
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; } // ให้ user บอก วันเกิดดีกว่าบอกอายุเพราะอายุมันเปลี่ยนทุกปี แต่วันเกิดจะเหมือนเดิม
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; } // คือ User class มีความสัมพันธ์กับ Photo class (1 user มีหลาย photo คือได้ความสัมพันธ์แบบ 1-to-many)
        // สร้าง method ใน entity class
        public int GetAge() // หลังจาก Get คือ Age แล้ว return int ตัว AutoMapper จะทำการ run code ใน GetAge แล้วเอาค่ามาใส่ Age (ใน MemberDto เลย)
        {
            return DateOfBirth.CalculateAge();
        }
    }
}