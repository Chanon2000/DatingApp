using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    // 12.2 สร้าง class ของ TokenService
    public class TokenService : ITokenService
    {
    // #13. Adding the create token logic
        private readonly SymmetricSecurityKey _key; 
        // SymmetricSecurityKey เป็นประเภทของการ encryption ที่ 1 key จะทำทั้ง encrypt และ decrypt
    
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            // เนื่องจากว่า SymmetricSecurityKey รับ พาราเป็น ฺByte[] เลยต้อง Encoding ซึ่งจะ GetBytes มาจาก TokenKey ซึ่งเป็น property ของ config
        }

        public string CreateToken(AppUser user)
        {
            // เติม claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };
            
            // สร้าง Credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // อธิบายว่า Token เราจะหน้าตาเป็นยังไง
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //กำหนดสิ่งที่อยู่ใน token
                Subject = new ClaimsIdentity(claims), // ใส่ claims ของเราเข้าไป
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds // ใส่ creds ของเราเข้าไป
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // ทำการสร้าง token
            var token = tokenHandler.CreateToken(tokenDescriptor); //ใส่ tokenDescriptor

            // ทำการ return token ที่ถูกเขียนแล้ว
            return tokenHandler.WriteToken(token);
        }
    }
}