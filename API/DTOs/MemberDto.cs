using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs
{
    public class MemberDto
    {
        // เราจะใช้ Dto เข้ามาแก้ปัญหาการวนลูปดึงข้อมูล
        public int Id { get; set; } 
        public string UserName { get; set; }
        public int Age { get; set; } // return อายุไปแทน
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } //  = DateTime.Now; ไม่ต้องกำหนดค่าอะไร
        public DateTime LastActive { get; set; } // = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }
}