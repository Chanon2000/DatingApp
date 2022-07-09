using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        // คือช่วยเรา map object ไปอีกอย่างนึง
        public AutoMapperProfiles()
        {
            // สร้าง map function
            CreateMap<AppUser, MemberDto>() // เพื่อ map AppUser เป็น MemberDto
            // เพื่อจะเอาค่า Url จาก Photo ไปใส่ให้กับ photoUrl จาก MemberDto
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => 
                    src.Photos.FirstOrDefault(x => x.IsMain).Url))
                    // ไปที่ Photos collection แล้วเอา FirstOrDefault ที่เป็น IsMain และเอาแค่ค่า Url มา
                // ForMember หมายถึง property ใหนที่คุณอยากจะจัดการ
                // dest = destination เอาค่าที่จัดการไปใส่ที่ไหน
                // opt = option 
                // src คือ บอกว่าจะ MapFrom จากอะไร
                // เรามาเพิ่ม ForMember เพื่อมาคำนวณ Age ตรงนี้แทน
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            // เราต้อง add ไปที่ application service extensions
        }
    }
}