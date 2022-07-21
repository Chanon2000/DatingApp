using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{ // สร้าง class LogUserActivity มา เพื่อใช้ ActionFilter เพื่อทำการทำบางอย่าง ก่อนหรือหลังจากที่ request ทำการ executing
    public class LogUserActivity : IAsyncActionFilter // คลิก Implement interface จะได้ OnActionExecutionAsync มา
    {
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        // มี 2 parameters ให้ใช้
        // next จะ return ActionExecutedContext คือ context ที่ได้หลังจาก execute เสร็จ
        // ส่วน context คือ context ที่ได้ก่อน execute
        {
            var resultContext = await next(); // เราสามารถใช้ next() เพื่อทำการ execute (execute สิ่งที่ user ยิงมาเพื่อทำ เช่น ใน GetUsers endpoint) และหลังจาก execute ก็ให้ทำอะไรบางอย่างหลังจาก execute เสร็จ ผ่าน resultContext ที่ได้ (นั้นคือ code หลังจากบรรทัดนี้)
            
            // ถ้า user ส่ง token มา แล้ว เราทำการตรวจสอบ token แล้ว IsAuthenticated จะเป็น true
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            // ถ้าทำการ IsAuthenticated แล้ว ให้ทำการให้ update ค่าของ last active property
            var userId = resultContext.HttpContext.User.GetUserId();
            // เราต้องการจะเข้า repository ของเรา ซึ่งจะเข้าถึงตรงนี้ได้โดยใช้ service locator pattern นี้ (เนื่องจากเราจะใช้ method นี้ที่ program.cs ด้วย)
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>(); // คือเราต้องการเข้าถึง User Repository
            var user = await repo.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}