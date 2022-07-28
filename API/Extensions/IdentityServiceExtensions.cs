using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            // services.AddIdentity
            // ถ้าเราทำแบบ MVC ที่ client serve ด้วย dotnet server แบบนั้นเราจะใช้สิ่งที่เรียกว่า "razor pages"
            // สิ่งนี้จะให้ full setup เลย เช่น page ที่ต้องการ, cookie based authentication เพราะทุกอย่าง generate โดย dotnet server
            // แต่ที่เราใช้คือ single page application โดย angular แล้วใช้ token based authorization
            services.AddIdentityCore<AppUser>(opt => 
            {
                // ใส่ option
                opt.Password.RequireNonAlphanumeric = false; // ปิดการ RequireNonAlphanumeric ออก
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>() // อย่าลืมใส่ RoleManager type ก่อนด้วย แล้วค่อยใส่ AppRole type ซึ่งเป็น type ที่เราจะใช้
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>() // พวก type เช่น RoleValidator, RoleManager, SignInManager ที่ใส่เข้าก่อน type ของคุณ เป็น type ของ Services เอง ที่บังคับให้คุณต้องใส่ก่อน
                .AddEntityFrameworkStores<DataContext>(); // ทำการ setup tables ต่างๆ ของidentity

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            services.AddAuthorization(opt => 
            {
                // RequireAdminRole คือ policy name
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

            return services;
        }
    }
}