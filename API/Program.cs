using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


// ทุก app .net ต้องมี Program.cs ซึ่งมี class ที่มี main method // จะทำงานตรงนี้เป็นที่แรก
namespace API
{
    public class Program
    {
        public static void Main(string[] args) // main // ที่แรกที่ dotnet จะเข้ามาดู
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args) 
            // CreateDefaultBuilder คือสิ่งสำคัญเพราะทำหลายอย่าง 
                .ConfigureWebHostDefaults(webBuilder =>
                { // ในนี้คือ อะไรที่จะใช้อีกเมื่อ เริ่มต้น run application
                    webBuilder.UseStartup<Startup>(); // มีการใช้ Startup class
                });
    }
}
