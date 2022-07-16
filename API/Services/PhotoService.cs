using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        // IOptions เพื่อ get configuretion ต่างๆ (จาก Microsoft.Extensions.Options)
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            // cloud account
            var acc = new Account // คุณต้อง เรียงลำดับแต่ละ property ให้ถูก (เพราะ cloudinary ต้องการแบบนี้)
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            // Length เป็น property ใน file
            if (file.Length > 0) 
            {
                // เพราะ stream จะเป็นสิ่งที่เราจะทิ้งทันทีเมื่อเราใช้เสร็จ (เลยเติม using)
                // OpenReadStream ไม่ใช่ async method เลยไม่ต้องใส่ await
                using var stream = file.OpenReadStream(); // เรากำลังทำให้การ get file เป็นแบบ stream ของ data
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream), 
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                    // เพื่อให้แน่ใจว่ารูปภาพจะมีขนาดตาม aspect ratio นี้
                    // Crop("fill") ตัดให้เป็นสี่เหลี่ยม
                    // Gravity("face") ให้ focus ไปที่ หน้า
                };
                // uploadResult จะเอาตัวแปรเก็บค่าที่ได้จากการไป upload ลง cloudinary // จะมี property ชื่อ SecureUri ซึ่งคือ url ที่อยู่ของรูป
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }
        // สังเกตได้ว่า logic ไม่ได้ซัพซ้อนเลย
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}