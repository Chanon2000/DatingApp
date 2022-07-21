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
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key; 
    
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                // เพราะ Claim เป็น string คุณเลยต้องเปลี่ยน Id เป็น string
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                // โดยเมื่อคุณนำไปแปลง token ไปแปลงที่ jwt.io จะได้ payload เป็น
                // {
                //     "nameid": "1",
                //     "unique_name": "lisa",
                //     "nbf": 1658416743,
                //     "exp": 1659021543,
                //     "iat": 1658416743
                // }
            };
            
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // ทำการสร้าง token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // ทำการ return token ที่ถูกเขียนแล้ว
            return tokenHandler.WriteToken(token);
        }
    }
}