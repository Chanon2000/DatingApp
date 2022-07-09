using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args) // เปลี่ยนให้เป็น async method
        {
           var host = CreateHostBuilder(args).Build();
            // สร้าง scope เพื่อให้เข้าถึง service ของเราได้ เพราะเราจะใช้ Seed Method ที่อยู่ใน DataContext service ของเรา
           using var scope = host.Services.CreateScope();
           var services = scope.ServiceProvider;
           // เนื่องจากตรงนี้เราไม่ได้เข้าถึง global exception handler (เราอยู่ที่ Main method ซึ่งอยู่นอก middleware ) ที่เราทำเอาไว้ เลย try catch เลย
           try
           {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync(); // ทำการ Migrate database ให้เรา (คล้ายการทำ dotnet ef database update)
                // ในอนาคตคุณแค่ restart application มันก็จะ migration ให้เราเลย
                // ถ้าเรา drop database สิ่งที่เราต้องทำคือ restart app มันก็จากสร้างใหม่ทันที
                await Seed.SeedUsers(context);
           }
           catch (Exception ex)
           {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
           }
            
           await host.RunAsync(); // หลังจากทำสิ่งที่เราต้องการเสร็จอย่าลืมสั่ง run ด้วย
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) 
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
