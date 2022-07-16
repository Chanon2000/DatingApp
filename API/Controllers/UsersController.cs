using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();

            return Ok(users);
        }
    
        [HttpGet("{username}", Name = "GetUser")] // Name ตั้งชื่อ route name ให้ route นี้
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            // เราต้องเอา username จาก claims principle
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            // GetUserByUsernameAsync อย่าลืมเมื่อเราใช้ func นี้ มันเอา photos ของ user มาด้วย

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri, // .SecureUrl.AbsoluteUri คือ property ที่เราจะได้กลับมา
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                // เราจะมาใช้ created route เพื่อ generate response ที่ถูกต้อง
                // Created(), Created อื่นๆ ถ้าคุณลองอ่านที่ popup จะเห็นว่ามันใช้ยากไป
                // return CreatedAtRoute("GetUser", _mapper.Map<PhotoDto>(photo)); // เนื่องจาก GetUser route ต้องใส่ username ด้วย เราเลยต้องใช้ overload ใหม่ (คือ method ชื่อเดียวกันแต่ใส่ parameter ไม่เหมือนกัน) // GetUser ต้องใส่ parameter
                // GetUser คือค่า routeName
                return CreatedAtRoute("GetUser", new { Username = user.UserName}, _mapper.Map<PhotoDto>(photo));
                // ใส่ new {} เพราะว่า second มันจะรับเป็น obj

                // สิ่งที่คุณจะได้คือ status 201 และมีการใส่ Location ที่ header โดยเอา route ที่คุณใส่เข้าไปไปวางไว้ (ประมาณว่า เพื่อบอกว่า คุณจะเห็นข้อมูลที่สร้างใหม่นี้ได้จากการยิง api เส้นไหน)
                // ที่นี้คือ https://localhost:5001/api/Users/lisa
            }

            return BadRequest("Problem adding photo"); // ถ้า fail ก็จะ return ตรงนี้
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            // การที่เรา get username จาก token ทำให้เรามั้นใจได้ว่านี้คือ user คนนี้จริงๆ คนเดียวกับ user ที่ยิงมา (เรามั้นใจข้อมูลที่มาจาก token)

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId); // ไม่ใช่ async เพราะว่าเราได้ข้อมูลจาก user มาเก็บที่ memory แล้ว (เราไม่ได้กำลังจะไปดึงข้อมูลจาก database)

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain); // เอา record ที่ IsMain == true
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent(); // เมื่อเราทำการ update แต่ไม่มีอะไรต้อง response ให้ NoContent() ไม่เลย

            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("You cannot delete your main photo");

            if (photo.PublicId != null)
            {
                // สิ่งที่เราจะได้กลับคือ CloudinaryDotNet.Actions.DeletionResult
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}