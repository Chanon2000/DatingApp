using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        // #4.2 inject IConfiguration
        //เปลี่ยน configuration เป็น config เพื่อให้ง่ายในการเขียน
        private readonly IConfiguration _config; // ใส่ _ เพื่อเพิ่มความแตกต่างกับตัวแปรอื่น
        public Startup(IConfiguration config) // configuration ถูก inject ลง Startup 
        // IConfiguration ถูก inject ลง Startup ทำให้เราเข้าถึง IConfiguration ได้โดยตัวแปร _config
        {
            _config = config; // เมื่อเติม _ ทำให้ไม่ต้องเติม this ( this.config => _config ) 
        }

        // ไม่ชอบแบบนี้
        // public IConfiguration Configuration { get; } 

        // This method gets called by the runtime. Use this method to add services to the container.
        // จะใช้อ้างถึง dependency injection container
        // คือถ้าคุณอยากจะสร้าง service หรือ class ที่สามารถใช้ใน พื้นที่อื่นได้ เราต้อง add เข้ามาในนี้
        public void ConfigureServices(IServiceCollection services)
        { // เพื่อ add พวก class หรือ method ที่จะใช้ในหลายๆที่ลงตรงนี้
            // #4.1 สร้าง connection string ไปที่ database
            services.AddDbContext<DataContext>(options =>
            // DataContext ชื่อของ class ที่เราสร้าง
            // options
            {
                // #4.3 ใส่ _config ลง UseSqlite
                // ทำการ passing string เพื่อเชื่อมต่อกับ database
                // ทำ _config มาใช้ (ที่ไฟล์ appsettings.Development.json )
                // GetConnectionString(name) เป็น shorthandของ GetSection("ConnectionStrings")[name]
                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ใช้ในการตั้งค่า HTTP request pipeline (คล้ายกับ middleware ที่ req หรือ res จะผ่านก่อน)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // เริ่มแรกก็ check ก่อนว่าอยู่ใน mode Devหรือป่าว
            if (env.IsDevelopment()) // เมื่ออยู่ใน dev env
            {
                app.UseDeveloperExceptionPage(); // นั้นคือเมื่อเราเจอปัญหา แล้วอยู่ใน mode dev จะเข้าหน้า Exception Page
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection(); //เมื่อเราใช้ http address เราก็จะ redirect to http endpoint

            app.UseRouting(); // เช่น WeatherForecast // ทำให้เราสามารถใส่เร้าที่ browser แล้วเข้าไปที่ controller ได้

            app.UseAuthorization(); // ตอนนี้เราอาจไม่ได้ใช้มันมากเพราะว่าเราไม่ได้ตั้งค่าเกี่ยวกับ authorization


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // คือเก็บ endpoint แล้วมา map กับ controller ตรงนี้
                                            // ซึ่งตรงนี้จะดูใรน controller ว่า controller ไหนที่ตรงกับ endpoint
            });
        }
    }
}
