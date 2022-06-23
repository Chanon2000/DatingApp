using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace API
{
    public class Startup
    {
        // #4.2 inject IConfiguration
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // จะใช้อ้างถึง dependency injection container
    // A. ที่ inject class
        public void ConfigureServices(IServiceCollection services)
        {
    // #16.2 cut service ใน นี้ ไปไว้ที่ extension
            // .......

    // #16.4 เรียก applicationservice ที่ stratup
            services.AddApplicationServices(_config);


            services.AddControllers();
            services.AddCors();
    
    // #16.6 cut service ใน นี้ ไปไว้ที่ extension
            // .......
            services.AddIdentityServices(_config);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

        // #5. adding cors support in the API
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200")); // ควรอยู่ระหว่าง UseRouting กับ UseEndpoints และก่อน UseAuthorization

        // #15.3 เพิ่ม middlewareที่ Configure //(อย่าลืมลำดับสำคัญมากต้องใส่ก่อน UseAuthorization)
            app.UseAuthentication();

            app.UseAuthorization(); // ตอนนี้เราอาจไม่ได้ใช้มันมากเพราะว่าเราไม่ได้ตั้งค่าเกี่ยวกับ authorization


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
