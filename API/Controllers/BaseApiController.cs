using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // class นี้คือที่ ที่ ทุก request ก่อนจะเข้า controller จะผ่านก่อน
    [ServiceFilter(typeof(LogUserActivity))] // ทำการใช้ action filter ที่นี้ โดยใส่ LogUserActivity type เข้าไป
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}