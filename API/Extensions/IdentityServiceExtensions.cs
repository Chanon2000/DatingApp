using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    // #16.5 ทำกับ Identity เหมือนขั้นตอนทั้งหมดก่อนหน้า (16.1, 16.2, 16.3, 16.4)
        // * copy มาวางอย่าลืมเป็น _config เป็น config ด้วย
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
        // #15.2 เพื่ม middlewareที่ ConfigureServices (AddAuthentication)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // เพิ่ม middleware
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    { // ทำการใส่ options
                        ValidateIssuerSigningKey = true, // บอกให้กำการ validate token
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // ValidateIssuer, ValidateAudience จะมาจาก angular
                    };
                });


            return services;
        }
    }
}