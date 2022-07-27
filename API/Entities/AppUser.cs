using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int> // เราไม่ต้อง install อะไรเพิ่มเลยเพราะ dotnet มาพร้อมกับ Identity class
    // <int> เพื่อใช้ int เป็ฯ primary key แทน string ที่เป็น default
    {
        // public int Id { get; set; } // field พวกนี้ ไม่จำเป็นแล้ว เพราะว่าเราจะใช้จาก IdentityUser ที่ implement มา
        // public string UserName { get; set; } // "UserName" คือ username key ที่ใช้ใน IdentityUser อยู่แล้ว
        // public byte[] PasswordHash { get; set; }
        // public byte[] PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; } 
        public ICollection<UserLike> LikedByUsers { get; set; }
        public ICollection<UserLike> LikedUsers { get; set; }

        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}