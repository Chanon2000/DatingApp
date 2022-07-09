using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        // คือช่วยเรา map object ไปอีกอย่างนึง
        public AutoMapperProfiles()
        {
            // สร้าง map function
            CreateMap<AppUser, MemberDto>(); // เพื่อ map AppUser เป็น MemberDto
            CreateMap<Photo, PhotoDto>();
            // เราต้อง add ไปที่ application service extensions
        }
    }
}