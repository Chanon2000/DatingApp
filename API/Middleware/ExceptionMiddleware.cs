using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(
            RequestDelegate next,  // RequestDelegate คือ อะไรจะมาต่อใน middleware pipeline
            ILogger<ExceptionMiddleware> logger,  // ILogger เพื่อ log ออกมาที่ terminal
            IHostEnvironment env // IWebHostEnvironment เพื่อเอามาดูว่าตอนนี้อยู่บน production, dev
        )
        {
            _logger = logger;
            _env = env;
            _next = next;

        }

        public async Task InvokeAsync(HttpContext context) // context คือ http request จริงๆที่ยิงเข้ามา  
        {
            try
            {
                await _next(context); // ส่งให้ middleware อันต่อไป
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); // ถ้าไม่ทำอันนี้เราก็จะไม่เห็น error ของเราใน terminal
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                // ถ้าอยู่ใน development mode ทำ response แบบนี้
                // ex.StackTrace?.ToString() ใส่ ? เพื่อป้องกัน exception เมื่อ ex.StackTrace เป็น null
                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase}; // set option

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }

            // หน้าสนใจคือ ถ้า middleware อันใหนเกิด exception มันจะ throw ขึ้นไปด้านบนเรื่อยๆจนกว่าจะเจอ บางอย่าง ที่จัดการ exception ได้
        }
    }
}