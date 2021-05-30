using Microsoft.AspNetCore.Mvc;
// #7.สร้าง BaseApiController ที่รับ ControllerBase มา
namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}