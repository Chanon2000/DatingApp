using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    // 16. Adding extension methods
    public static class ApplicationServiceExtensions
    { // static ทำให้เราใช้classนี้โดยไม่ต้องสร้าง instance
    // #16.1 สร้าง construtor
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        { // return เป็น IServiceCollection
    // 12.3 inject ITokenService
        // AddSingleton มันจะยังคงใช้งานได้จนกว่า app จะถูกหยุด
        // ซึ่ง Token เราต้องการใช้งานแค่ต้อนที่มันส่งมาเท่านั้น (AddSingleton จึงไม่ควรใช้)
        // AddScoped จะ inject ตอนที่ใช้แล้วเมื่อใช้เสร็จก็จะ disposed  (เหมาะกับ http req ที่สุด (เนื่องจากเราจะใช้ token ที่ apicontroller))
        // AddTransient
            services.AddScoped<ITokenService, TokenService>();
            // ที่เราต้องกำหนด interface มีเหตุผลหลักคือ (ITokenService)
                // เพื่อ testing 

                
    // #4.1 สร้าง connection string ไปที่ database
            services.AddDbContext<DataContext>(options =>
            // DataContext ชื่อของ class ที่เราสร้าง
            // options
            {
    // #4.3 ใส่ _config ลง UseSqlite
                // ทำการ passing string เพื่อเชื่อมต่อกับ database
                // ทำ _config มาใช้ (ที่ไฟล์ appsettings.Development.json )
                // GetConnectionString(name) เป็น shorthandของ GetSection("ConnectionStrings")[name]
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

    // #16.3 return services
            return services;
        }
    }
}